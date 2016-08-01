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
#load "Gemini.fs"

open Countersoft.Gemini.Commons.Dto
open TimeLogger.Gemini

// setup the service manager with your credentials
let svc = service "https://localhost" "user" "apikey"
let cmd = Commands.init svc
// helper method for extracting id and name of the project
let view (x : ProjectDto) = x.Entity.Id, x.Entity.Name

// print list of the projects available
svc.Projects.GetProjects()
|> Seq.map view
|> Seq.iter (printfn "%A")

let my_project = 19
// create new story
let tasks_story = cmd.submit_issue my_project "important tasks"

// the following example is a special flow for the case where you have a story and you add new tasks to it.
// creates an issue adds time and closes the issue.
cmd.submit_closed_issue tasks_story.Id 0 15 "2016-01-01" "make some coffee"
