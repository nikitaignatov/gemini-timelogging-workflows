namespace TimeLogger

module Gemini = 
    type time = System.DateTime
    
    type NULL<'a when 'a : (new : unit -> 'a) and 'a : struct and 'a :> System.ValueType> = System.Nullable<'a>
    
    module Const = 
        let Closed = 70
        let Billable = 30
    
    module Create = 
        open Countersoft.Gemini.Commons.Dto
        open Countersoft.Gemini.Commons.Entity
        
        let time_entry issue_id user_id project_id message hours minutes date = 
            let now = date
            new IssueTimeTracking(IssueId = issue_id, TimeTypeId = NULL Const.Billable, Hours = hours, Minutes = minutes, Comment = message, EntryDate = now, UserId = user_id, 
                                  Active = true, Archived = false, Deleted = false, Created = now, ProjectId = project_id)
        
        let issue parent_id project_id message = new Issue(ProjectId = project_id, ParentIssueId = parent_id, Title = message)
    
    open Countersoft.Gemini.Api
    
    type Service = ServiceManager
    
    let service url user key = new Service(url, user, "", key)
    
    module Get = 
        let user (svc : Service) = svc.Item.WhoAmI()
        let issue (svc : Service) id = svc.Item.Get id
    
    module Submit = 
        let log_time (svc : Service) time = svc.Item.LogTime time
        let issue (svc : Service) issue = svc.Item.Create issue
        let issue_new (svc : Service) project_id title = Create.issue (new NULL<int>()) project_id title |> issue svc
        
        let closed_issue (svc : Service) parent_id hours minutes (dateValue : string) (message : string) = 
            let date = time.Parse dateValue
            let text = message.Trim()
            let parent = Get.issue svc parent_id
            let project_id = parent.Project.Id
            let user_id = Get.user(svc).Entity.Id
            let issue = Create.issue (NULL parent_id) project_id text |> issue svc
            printfn "%A %A %A %A %A %A %A" issue.Id project_id user_id text hours minutes date
            let time = Create.time_entry issue.Id project_id user_id text hours minutes date |> log_time svc
            issue.Entity.StatusId <- Const.Closed
            svc.Item.Update issue.Entity
    
    module Commands = 
        open Countersoft.Gemini.Commons.Dto
        
        type Command = 
            { submit_issue : int -> string -> IssueDto
              submit_closed_issue : int -> int -> int -> string -> string -> IssueDto }
        
        let init (svc : Service) = 
            { submit_closed_issue = Submit.closed_issue svc
              submit_issue = Submit.issue_new svc }
