Public Delegate Sub FeatureInitializer(feature As IFeature)
Public Interface IFeature
    Inherits IMetaphorEntity
    ReadOnly Property Location As ILocation
    Property Destination As ILocation
    Property Twin As IFeature
End Interface
