Friend Module LocationInitializationExtensions
#Region "Pier"
    Friend Function InitializePier(chosenName As String) As Persistence.LocationInitializer
        Return Sub(room)
                   room.CreateN00b(chosenName)
               End Sub
    End Function
#End Region
End Module
