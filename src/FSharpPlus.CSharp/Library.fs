namespace FSharpPlusCSharp
open System.Runtime.CompilerServices
open System
open System.Collections.Generic
open FSharpPlus
module Internals=
    module Result=
        /// Wraps a function, encapsulates any exception thrown within to a Result
        let inline protect f x = 
            try
                Ok (f x)
            with e -> Error e
        /// Attempts to cast an object.
        /// Stores the cast value in Ok if successful, otherwise stores the exception in Error
        let inline cast (o: obj) = protect unbox o
        
open Internals
/// Extensions around Actions and Funcs
[<Extension>]
type Funcs =
    /// Converts an action to a function returning Unit
    [<Extension>]
    static member ToFunc (a: Action) =
        Func<_>(a.Invoke)

    /// Converts an action to a function returning Unit
    [<Extension>]
    static member ToFunc (a: Action<_>) =
        Func<_,_>(a.Invoke)
  
    /// Converts an action to a function returning Unit
    [<Extension>]
    static member ToFunc (a: Action<_,_>) =
        Func<_,_,_>(curry a.Invoke)

    /// Converts an action to a function returning Unit
    [<Extension>]
    static member ToFunc (f: Action<_,_,_>) =
        Func<_,_,_,_>(fun a b c -> f.Invoke(a,b,c))

    /// Converts an uncurried function to a curried function
    [<Extension>]
    static member Curry (f: Func<_,_,_>) =
        Func<_,Func<_,_>>(fun a -> Func<_,_>(fun b -> f.Invoke(a,b)))

    /// Converts an uncurried function to a curried function
    [<Extension>]
    static member Curry (f: Func<_,_,_,_>) =
        Func<_,Func<_,Func<_,_>>>(fun a -> Func<_,Func<_,_>>(fun b -> Func<_,_>(fun c -> f.Invoke(a,b,c))))

    /// Converts an action with 2 arguments into an action taking a 2-tuple
    [<Extension>]
    static member Tuple (f: Action<_,_>) =
        Action<_>(fun (a,b) -> f.Invoke(a,b))

    /// Converts an action with 3 arguments into an action taking a 3-tuple
    [<Extension>]
    static member Tuple (f: Action<_,_,_>) =
        Action<_>(fun (a,b,c) -> f.Invoke(a,b,c))

    /// Converts an action with 4 arguments into an action taking a 4-tuple
    [<Extension>]
    static member Tuple (f: Action<_,_,_,_>) =
        Action<_>(fun (a,b,c,d) -> f.Invoke(a,b,c,d))

    /// Converts an action taking a 2-tuple into an action with 2 parameters
    [<Extension>]
    static member Untuple (f: Action<_ * _>) =
        Action<_,_>(fun a b -> f.Invoke(a,b))

    /// /// Converts an action taking a 3-tuple into an action with 3 parameters
    [<Extension>]
    static member Untuple (f: Action<_ * _ * _>) =
        Action<_,_,_>(fun a b c -> f.Invoke(a,b,c))

    /// Converts an action taking a 4-tuple into an action with 4 parameters
    [<Extension>]
    static member Untuple (f: Action<_ * _ * _ * _>) =
        Action<_,_,_,_>(fun a b c d -> f.Invoke(a,b,c,d))

    /// Composes two functions.
    /// Mathematically: f . g
    [<Extension>]
    static member Compose (f: Func<_,_>, g: Func<_,_>) =
        Func<_,_>(fun x -> f.Invoke(g.Invoke(x)))

    /// Composes two functions (forward composition).
    /// Mathematically: g . f
    [<Extension>]
    static member AndThen (f: Func<_,_>, g: Func<_,_>) =
        Func<_,_>(fun x -> g.Invoke(f.Invoke(x)))

[<Extension>]
type Option =
    static member Cast(o:obj) : 'a option =
        try
            Some (unbox o)
        with _ -> None
    static member None() : 'a option = None

    [<Extension>]
    static member HasValue o = Option.isSome o

    [<Extension>]
    static member ToNullable o =
        match o with
        | Some x -> Nullable x
        | _ -> Nullable()

    [<Extension>]
    static member ToOption (n: Nullable<_>) =
        if n.HasValue
            then Some n.Value
            else None

    [<Extension>]
    static member ToOption v = 
        match box v with
        | null -> None
        | :? DBNull -> None
        | _ -> Some v

    [<Extension>]
    static member Some (a:'a) : 'a option = Option.Some a

    [<Extension>]
    static member Match (o, ifSome: Func<_,_>, ifNone: Func<_>) =
        match o with
        | Some x -> ifSome.Invoke x
        | _ -> ifNone.Invoke()

    [<Extension>]
    static member Match (o, ifSome: Func<_,_>, ifNone) =
        match o with
        | Some x -> ifSome.Invoke x
        | _ -> ifNone

    [<Extension>]
    static member Match (o, ifSome: Action<_>, ifNone: Action) =
        match o with
        | Some x -> ifSome.Invoke x
        | _ -> ifNone.Invoke()

    [<Extension>]
    static member Do (o, f: Action<_>) =
        match o with
        | Some v -> f.Invoke v
        | _ -> ()

    /// Gets the option if Some x, otherwise the supplied default value.
    [<Extension>]
    static member OrElse (o, other) =
        match o with
        | Some x -> Some x
        | _ -> other

    [<Extension>]
    static member OrElseLazy (o, other: _ Lazy) =
        match o with
        | Some x -> Some x
        | _ -> other.Force()

    [<Extension>]
    static member GetOrElse (o, other) =
        match o with
        | Some x -> x
        | _ -> other

    [<Extension>]
    static member GetOrElse (o, other: _ Func) =
        match o with
        | Some x -> x
        | _ -> other.Invoke()

    [<Extension>]
    static member GetOrDefault (o: Option<_>) =
        match o with
        | Some x -> x
        | _ -> Unchecked.defaultof<_>

    [<Extension>]
    static member ToChoice (o, other) =
        match o with
        | Some v -> Choice1Of2 v
        | _ -> Choice2Of2 other

    [<Extension>]
    static member ToResult (o, other) =
        match o with
        | Some v -> Ok v
        | _ -> Result.Error other

    /// Converts the option to a list of length 0 or 1
    [<Extension>]
    static member ToFSharpList o = Option.toList o

    /// Converts the option to an array of length 0 or 1
    [<Extension>]
    static member ToArray o = Option.toArray o

    /// Transforms an option value by using a specified mapping function
    [<Extension>]
    static member Select (o, f: Func<_,_>) = Option.map f.Invoke o

    /// Invokes a function on an optional value that itself yields an option
    [<Extension>]
    static member SelectMany (o, f: Func<_,_>) = Option.bind f.Invoke o

    /// Invokes a function on an optional value that itself yields an option,
    /// and then applies a mapping function
    [<Extension>]
    static member SelectMany (o, f: Func<_,_>, mapper: Func<_,_,_>) =
      let mapper = liftA2 (curry mapper.Invoke)
      let v = Option.bind f.Invoke o
      mapper o v

    /// <summary>
    /// Evaluates the equivalent of <see cref="System.Linq.Enumerable.Aggregate"/> for an option
    /// </summary>
    [<Extension>]
    static member Aggregate (o, state, f: Func<_,_,_>) =
        Option.fold (curry f.Invoke) state o

    /// Applies a predicate to the option. If the predicate returns true, returns Some x, otherwise None.
    [<Extension>]
    static member Where (o: _ option, pred: _ Predicate) =
      Option.filter pred.Invoke o

    static member SomeUnit = Some()

    static member ParseInt s : int32 option = tryParse s

    static member ParseDecimal s : Decimal option= tryParse s

    static member ParseDouble s : Double option = tryParse s

    static member ParseFloat s : Single option = tryParse s

    static member ParseInt16 s : Int16 option= tryParse s

    static member ParseInt64 s : Int64 option= tryParse s

    static member ParseByte s : byte option= tryParse s

    static member ParseDateTime s : DateTime option= tryParse s

    static member ParseDateTimeOffset s :DateTimeOffset option= tryParse s

[<Extension>]
type Choice =


    [<Extension>]
    static member Match (c, f1: Func<_,_>, f2: Func<_,_>) =
        match c with
        | Choice1Of2 x -> f1.Invoke x
        | Choice2Of2 y -> f2.Invoke y

    [<Extension>]
    static member Match (c, f1: Action<_>, f2: Action<_>) =
        match c with
        | Choice1Of2 x -> f1.Invoke x
        | Choice2Of2 y -> f2.Invoke y

    [<Extension>]
    static member Match (c, f1: Func<_,_>, f2: Func<_,_>, f3: Func<_,_>) =
        match c with
        | Choice1Of3 x -> f1.Invoke x
        | Choice2Of3 x -> f2.Invoke x
        | Choice3Of3 x -> f3.Invoke x

    [<Extension>]
    static member Match (c, f1: Action<_>, f2: Action<_>, f3: Action<_>) =
        match c with
        | Choice1Of3 x -> f1.Invoke x
        | Choice2Of3 x -> f2.Invoke x
        | Choice3Of3 x -> f3.Invoke x

    [<Extension>]
    static member SelectMany (o, f: Func<_,_>) =
        Choice.bind f.Invoke o

    [<Extension>]
    static member SelectMany (o, f: Func<_,_>, mapper: Func<_,_,_>) =
        let mapper = liftA2 (curry mapper.Invoke)
        let v = Choice.bind f.Invoke o
        mapper o v

    [<Extension>]
    static member Select (o, f: Func<_,_>) = Choice.map f.Invoke o

    //[<Extension>]
    //static member SelectSecond (o, f: Func<_,_>) = Choice.mapSecond f.Invoke o

    // constructors

    static member New1Of2<'T1,'T2> (a: 'T1) : Choice<'T1,'T2> = Choice1Of2 a
    static member New2Of2<'T1,'T2> (b: 'T2) : Choice<'T1,'T2> = Choice2Of2 b

    static member New1Of3<'T1,'T2,'T3> (a: 'T1) : Choice<'T1,'T2,'T3> = Choice1Of3 a
    static member New2Of3<'T1,'T2,'T3> (a: 'T2) : Choice<'T1,'T2,'T3> = Choice2Of3 a
    static member New3Of3<'T1,'T2,'T3> (a: 'T3) : Choice<'T1,'T2,'T3> = Choice3Of3 a

[<Extension>]
type Result =

    /// Attempts to cast an object.
    /// Stores the cast value in Ok if successful, otherwise stores the exception in Error
    static member Cast (o: obj) = Result.cast o

    [<Extension>]
    static member ToOption c = match c with | Ok a -> Some a | _ -> None

    [<Extension>]
    static member Match (c, f1: Func<_,_>, f2: Func<_,_>) =
        match c with
        | Ok x -> f1.Invoke x
        | Result.Error y -> f2.Invoke y

    [<Extension>]
    static member Match (c, f1: Action<_>, f2: Action<_>) =
        match c with
        | Ok x -> f1.Invoke x
        | Result.Error  y -> f2.Invoke y

    [<Extension>]
    static member SelectMany (o, f: Func<_,_>) =
        Result.bind f.Invoke o

    [<Extension>]
    static member SelectMany (o, f: Func<_,_>, mapper: Func<_,_,_>) =
        let mapper = liftA2 (curry mapper.Invoke)
        let v = Result.bind f.Invoke o
        mapper o v

    [<Extension>]
    static member Select (o, f: Func<_,_>) = Result.map f.Invoke o

    [<Extension>]
    static member SelectError (o, f: Func<_,_>) = Result.mapError f.Invoke o

    // constructors

    static member NewOk<'T1,'T2> (a: 'T1) : Result<'T1,'T2> = Ok a
    static member NewError<'T1,'T2> (b: 'T2) : Result<'T1,'T2> = Result.Error b


[<Extension>]
type FSharpList =
    [<Extension>]
    static member Match (l, empty: Func<_>, nonempty: Func<_,_,_>) =
        match l with
        | [] -> empty.Invoke()
        | x::xs -> nonempty.Invoke(x,xs)

    [<Extension>]
    static member Choose (l, chooser: Func<_,_>) =
        List.choose chooser.Invoke l

    [<Extension>]
    static member TryFind (l, pred: _ Predicate) = 
        List.tryFind pred.Invoke l

    [<Extension>]
    static member TryFind (l, value) = 
        List.tryFind ((=) value) l

    [<Extension>]
    static member Cons (l, e) = e::l

    static member Create([<ParamArray>] values: 'T1 array) =
        Seq.toList values

    [<Extension>]
    static member ToFSharpList s = Seq.toList s

[<Extension>]
type FSharpSet =
    static member Create([<ParamArray>] values: 'T1 array) =
        set values

    [<Extension>]
    static member ToFSharpSet values = set values

[<Extension>]
type FSharpMap =
    static member Create([<ParamArray>] values) =
        Map.ofArray values

    [<Extension>]
    static member ToFSharpMap values = Map.ofSeq values

[<Extension>]
type Dictionary =
  [<Extension>]
  static member TryGet (d : IDictionary<'key,'value>, key:'key) : 'value option = match d.TryGetValue key with | (true,v)->Some v | _ -> None 

[<Extension>]
type EnumerableEx =
    [<Extension>]
    static member FirstOrNone source = Seq.tryHead source

open System.Linq
module Enums=
    [<CompiledName("TryParse")>]
    let tryParse s :'a option = //todo: use f#+
        match System.Enum.TryParse (s) with
        | true, v -> Some v
        | _ -> None
    [<CompiledName("Parse")>]
    let parse s : 'a = //todo: use f#+
        System.Enum.Parse(typeof<'a>, s) :?> 'a
    [<CompiledName("GetValues")>]
    let getValues<'t> ()= 
        let values = System.Enum.GetValues (typeof<'t>) 
        Enumerable.Cast<'t> values //Array.unbox