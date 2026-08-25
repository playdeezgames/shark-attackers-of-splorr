Public Interface IFeaturesModel
    ReadOnly Property HasAny As Boolean
    ReadOnly Property AllVisible As IEnumerable(Of IFeatureModel)
    Sub ShowList()
End Interface
