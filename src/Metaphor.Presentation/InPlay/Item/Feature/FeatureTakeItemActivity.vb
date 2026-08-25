Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Module FeatureTakeItemActivity
    Friend Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, itemModel As IItemModel) As DialogSource
        Return Function()
                   itemModel.Take()
                   Return FeatureInventoryMenu.Launch(context, model, previous, featureModel).Invoke()
               End Function
    End Function
End Module
