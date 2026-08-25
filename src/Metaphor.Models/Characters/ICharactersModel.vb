Public Interface ICharactersModel
    ReadOnly Property HasAny As Boolean
    ReadOnly Property All As IEnumerable(Of ICharacterModel)
    Sub ShowList()
End Interface
