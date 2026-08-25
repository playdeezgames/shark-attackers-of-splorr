Public Interface ILocationModel
    ReadOnly Property AvailableVerbs As IEnumerable(Of IVerbModel)
    ReadOnly Property OtherCharacters As IEnumerable(Of ICharacterModel)
    ReadOnly Property Features As IFeaturesModel
    ReadOnly Property Characters As ICharactersModel
    ReadOnly Property Ground As IGroundModel
End Interface
