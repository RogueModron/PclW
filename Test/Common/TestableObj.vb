Imports PclWCommon


Public MustInherit Class TestableObj
    Public Property Id As String

    Public Property ClosedPolygonsRequired As Boolean
    Public Property InputAdapterFromPolygonSet As Object
    Public Property InputAdapterFromRegion As Object
    Public Property OutputAdapter As Object

    Public MustOverride Function GetAdaptedInputFromPolygonSet(
            ByVal input As PolygonSet) As Object

    Public MustOverride Function GetAdaptedInputFromRegion(
            ByVal input As Region) As Object

    Public MustOverride Function GetAdaptedOutputToPolygonSet(
            ByVal output As Object) As PolygonSet

    Public MustOverride Function GetDifference(ByVal subject As Object,
            ByVal clip As Object) As Object

    Public MustOverride Function GetIntersection(ByVal subject As Object,
            ByVal clip As Object) As Object

    Public MustOverride Function GetUnion(ByVal subject As Object,
            ByVal clip As Object) As Object

    Public MustOverride Function GetXor(ByVal subject As Object,
            ByVal clip As Object) As Object
End Class
