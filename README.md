# Gemini time logging

This is a tiny set of workflows on top of the [countersoft](https://github.com/countersoft) api.
The purpose is the ability to be able to log time directly from the VisualStudio with F# interactive, instead of using the website.


```fsharp
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
let submit_close x = Parent x |> cmd.submit_closed_issue
// define some fixed stories for the sprint
let consulting = submit_close 2
let testing = submit_close 4
let deployment = submit_close 8
let meetings = submit_close 16
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
```



## references: 
- Gemini website: https://www.countersoft.com/
- Gemini docs: http://docs.countersoft.com/
- Gemini .Net api: http://docs.countersoft.com/using-microsoft-net/

