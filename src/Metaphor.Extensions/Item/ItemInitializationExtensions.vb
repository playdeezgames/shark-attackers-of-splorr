Imports Metaphor.Persistence

Friend Module ItemInitializationExtensions
    Friend Sub InitializeSammich(item As IItem)
        item.SetCounter(Counters.STOMACH, 10)
        item.CreateVerb(VerbSubtypes.EAT, "Eat")
    End Sub
End Module
