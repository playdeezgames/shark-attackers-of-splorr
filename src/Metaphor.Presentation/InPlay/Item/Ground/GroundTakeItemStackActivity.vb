Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Module GroundTakeItemStackActivity
    Friend Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, itemStackModel As IItemStackModel, count As Integer) As DialogSource
        Return Function()
                   itemStackModel.Take(count)
                   Return GroundItemStackMenu.Launch(context, model, previous, itemStackModel).Invoke()
               End Function
    End Function

End Module
