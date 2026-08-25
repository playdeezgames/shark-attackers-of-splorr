Imports TGGD.Persistence

Public Class BaseModel(Of TEntity As IEntity)
    Implements IModel
    Protected ReadOnly Entity As TEntity
    Protected Sub New(entity As TEntity)
        Me.Entity = entity
    End Sub
End Class
