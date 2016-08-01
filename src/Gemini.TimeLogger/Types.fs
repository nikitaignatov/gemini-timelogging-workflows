module Types

type time = System.DateTime
    
type NULL<'a when 'a : (new : unit -> 'a) and 'a : struct and 'a :> System.ValueType> = System.Nullable<'a>

type parent_id = Parent of int      with member x.Value = match x with Parent v -> v
type project_id = Project of int    with member x.Value = match x with Project v -> v
type user_id = User of int          with member x.Value = match x with User v -> v
type issue_id = Issue of int        with member x.Value = match x with Issue v -> v
type hours = Hours of int           with member x.Value = match x with Hours v -> v
type minutes = Minutes of int       with member x.Value = match x with Minutes v -> v
type entry = Entry of string        with member x.Value = match x with Entry v -> v
    