namespace TimeLogger

module Gemini = 
    open Countersoft.Gemini.Api
    open Types
    
    type Service = ServiceManager
    
    let service url user key = new Service(url, user, "", key)
    
    module private Const = 
        let Closed = 70
        let Billable = 30
    
    module private Create = 
        open Countersoft.Gemini.Commons.Entity
        
        let time_entry (issue_id : issue_id) (project_id : project_id) (user_id : user_id) (message : entry) (hours : hours) (minutes : minutes) date = 
            new IssueTimeTracking(
                IssueId = issue_id.Value, 
                TimeTypeId = NULL Const.Billable, 
                Hours = hours.Value, 
                Minutes = minutes.Value, 
                Comment = message.Value, 
                EntryDate = date, 
                UserId = user_id.Value, 
                Active = true, 
                Archived = false, 
                Deleted = false, 
                ProjectId = project_id.Value)

        let issue parent_id (project_id : project_id) (message : entry) = 
            (new Issue(
                ProjectId = project_id.Value, 
                ParentIssueId = parent_id, 
                Title = message.Value, 
                Active = true, 
                Deleted = false, 
                Archived = false))

        let custom_field (id : issue_id) (project : project_id) (user : user_id) field value = 
            new CustomFieldData(
                ProjectId = project.Value, 
                IssueId = id.Value, 
                UserId = user.Value, 
                CustomFieldId = field, 
                Data = value)
        let comment (id : issue_id) (user_id : user_id) (message : entry) = 
            (new IssueComment(
                IssueId = id.Value, 
                UserId = user_id.Value, 
                Comment = message.Value))
    
    module private Get = 
        let user (svc : Service) = svc.Item.WhoAmI()
        
        let issue (svc : Service) (id : issue_id) = 
            let data = svc.Item.Get id.Value
            (Project data.Project.Id), data
    
    module private Submit = 
        let entry (svc : Service) time = svc.Item.LogTime time
        let submit_issue (svc : Service) issue = svc.Item.Create issue
        let issue_new (svc : Service) project_id title = Create.issue (new NULL<int>()) project_id title |> submit_issue svc
        
        let log_time (svc : Service) id hours minutes dateValue message = 
            let date = time.Parse dateValue
            let user_id = Get.user(svc).Entity.Id |> User
            let project_id, _ = Get.issue svc id
            svc.Item.IssueCommentCreate(Create.comment id user_id message) |> ignore
            Create.time_entry id project_id user_id message hours minutes date |> entry svc

        let sub_issue (svc : Service) (parent_id : parent_id) message = 
            let project_id, _ = Get.issue svc (Issue parent_id.Value)
            Create.issue (NULL parent_id.Value) project_id message |> submit_issue svc

        let close_issue (svc : Service) (id : issue_id)= 
            let _, data = Get.issue svc id
            data.Entity.StatusId <- Const.Closed
            svc.Item.Update data.Entity
        
        let add_custom_field (svc : Service) id (field, value) = 
            let project_id, _ = Get.issue svc id
            let user_id = Get.user(svc).Entity.Id |> User
            let data = Create.custom_field id project_id user_id field value
            svc.Item.CustomFieldDataUpdate data |> ignore
    
    module Commands = 
        open Countersoft.Gemini.Commons.Dto
        
        type Command = 
            { add_custom_field : issue_id -> (int * string) -> unit
              submit_issue : project_id -> entry -> IssueDto
              submit_sub_issue : parent_id -> entry -> IssueDto
              close_issue : issue_id -> IssueDto
              log_time : issue_id -> hours -> minutes -> string -> entry -> IssueTimeTrackingDto }
        
        let init (svc : Service) = 
            { submit_sub_issue = Submit.sub_issue svc
              submit_issue = Submit.issue_new svc
              close_issue = Submit.close_issue svc
              log_time = Submit.log_time svc
              add_custom_field = Submit.add_custom_field svc }
