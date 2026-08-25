Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class GroundTakeItemActivity
    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, itemModel As IItemModel) As DialogSource
        Return Function()
                   itemModel.Take()
                   Return GroundMenu.Launch(context, model, previous).Invoke()
               End Function
    End Function
End Class
