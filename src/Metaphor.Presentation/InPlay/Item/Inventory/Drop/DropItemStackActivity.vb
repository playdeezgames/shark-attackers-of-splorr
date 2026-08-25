Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Module DropItemStackActivity
    Friend Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, itemStackModel As IItemStackModel, count As Integer) As DialogSource
        Return Function()
                   itemStackModel.Drop(count)
                   Return InventoryItemStackMenu.Launch(context, model, previous, itemStackModel).Invoke()
               End Function
    End Function
End Module
