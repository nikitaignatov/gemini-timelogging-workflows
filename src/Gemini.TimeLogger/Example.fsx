(*

Get the DLL from countersoft website:
http://docs.countersoft.com/using-microsoft-net/
http://www.countersoft.com/gemini.zip

when you unzip the DLL files are in the following folder
Extras\Developer Samples\Gemini_API\Assemblies

*)

// reference the folder where you placed the asemplbies
#r @"..\lib\Countersoft.Foundation.Commons.dll"
#r @"..\lib\Countersoft.Gemini.Api.dll"
#r @"..\lib\Countersoft.Gemini.Commons.dll"
#r @"..\lib\RestSharp.dll"
// load the Gemini script
#load "Types.fs"
#load "Gemini.fs"

open Countersoft.Gemini.Commons.Dto
open TimeLogger.Gemini
open Types

// setup the service manager with your credentials
let svc = service "https://localhost" "user" "apikey"
let cmd = Commands.init svc

// helper method for extracting id and name of the project
// the following example is a special flow for the case where you have a story and you add new tasks to it.
// creates an issue adds time and closes the issue.
let submit_and_close x hours minutes date entry = 
    let m = cmd.submit_sub_issue (Parent x) entry
    let id = Issue m.Id
    cmd.log_time id hours minutes date entry |> ignore
    cmd.create_comment id entry |> ignore
    cmd.close_issue id

// define some fixed stories for the sprint
let consulting = submit_and_close 1024
let testing = submit_and_close 2048
let deployment = submit_and_close 512
let meetings = submit_and_close 1023
// custom fields 
let external_support_Q3 = 128, "support Q3"
let external_support_Q4 = 128, "support Q4"

// just plain timelogging.
cmd.log_time (Issue 16) (Hours 0) (Minutes 30) "2016-08-01 15:30" (Entry "Meeting with PM and customer")

// with new closed ticket
let issue = consulting (Hours 0) (Minutes 30) "2016-08-01 12:15" (Entry """Investingations some issues in database """)

// id of the new ticket
issue.Id
// add custom field to the ticket
cmd.add_custom_field (Issue issue.Id) external_support_Q3

// examples

// if you have 30 min meeting that occurs frequently or daily scrum
let status date summary = meetings (Hours 0) (Minutes 30) date (Entry summary)
let daily_scrum date summary = meetings (Hours 0) (Minutes 15) date (Entry (sprintf "DAILY SCRUM summary: %s" summary))

// they you can execute the commands as follows
daily_scrum "2016-08-01 12:15" "was working on the new feature, plan to continue today."


// playground 
let view (x : ProjectDto) = x.Entity.Id, x.Entity.Name

// print list of the projects available
svc.Projects.GetProjects()
|> Seq.map view
|> Seq.iter (printfn "%A")

let my_project = Project 19
// create new story
let tasks_story = cmd.submit_issue my_project (Entry "important tasks")
let p = svc.CustomFields.Get 61

p.Entity
