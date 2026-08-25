Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module InventoryExtensions
#Region "Sammich"
    <Extension>
    Friend Function CreateSammich(inventory As IInventory) As IItem
        Return inventory.CreateItem(ItemSubtypes.SAMMICH, "Half-eaten Sammich", AddressOf ItemInitializationExtensions.InitializeSammich)
    End Function
#End Region
End Module
