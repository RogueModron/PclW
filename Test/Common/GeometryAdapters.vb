Imports PclWCommon


Public Class InputAdapterFromPolygonSetToPolygonSetWithImplicitHoles
    Public Function GetAdaptedInput(ByVal input As PolygonSet) As PolygonSet
        'Clipper, KBool
        Dim inputPolygonSet As PolygonSet = CopyPolygonSet(input)
        Return inputPolygonSet
    End Function
End Class

Public Class InputAdapterFromPolygonSetToPolygonSetWithExplicitHoles
    Public Function GetAdaptedInput(ByVal input As PolygonSet) As PolygonSet
        'Gpc
        Dim inputPolygonSet As PolygonSet = CopyPolygonSet(input)
        Dim adaptedPolygonSet As New PolygonSet
        For Each polygon As Polygon In inputPolygonSet.Polygons
            If polygon.IsClockwise Then
                adaptedPolygonSet.Polygons.Add(polygon)
            Else
                adaptedPolygonSet.Holes.Add(polygon)
            End If
        Next
        Return adaptedPolygonSet
    End Function
End Class

Public Class InputAdapterFromPolygonSetToRegionWithClockwiseOuter
    Public Function GetAdaptedInput(ByVal input As PolygonSet) As Region
        'Bop, Geos, Ggl, TerraLib
        Dim inputPolygonSet As PolygonSet = CopyPolygonSet(input)
        Dim adaptedRegion As New Region
        Dim adaptedPolygonSet As PolygonSet = Nothing
        For Each polygon As Polygon In inputPolygonSet.Polygons
            If polygon.IsClockwise Then
                adaptedPolygonSet = New PolygonSet
                adaptedPolygonSet.Polygons.Add(polygon)
                adaptedRegion.PolygonSets.Add(adaptedPolygonSet)
            Else
                adaptedPolygonSet.Holes.Add(polygon)
            End If
        Next
        Return adaptedRegion
    End Function
End Class

Public Class InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter
    Public Function GetAdaptedInput(ByVal input As PolygonSet) As Region
        'Cgal, Gtl, PolyBoolean, MSSqlServerTypes
        Dim inputPolygonSet As PolygonSet = CopyPolygonSet(input)
        Dim adaptedRegion As New Region
        Dim adaptedPolygonSet As PolygonSet = Nothing
        For Each polygon As Polygon In inputPolygonSet.Polygons
            If polygon.IsClockwise Then
                adaptedPolygonSet = New PolygonSet
                adaptedPolygonSet.Polygons.Add(polygon)
                adaptedRegion.PolygonSets.Add(adaptedPolygonSet)
            Else
                adaptedPolygonSet.Holes.Add(polygon)
            End If
            polygon.Reverse()
        Next
        Return adaptedRegion
    End Function
End Class


Public Class InputAdapterFromRegionToPolygonSetWithImplicitHoles
    Public Function GetAdaptedInput(ByVal input As Region) As PolygonSet
        'Clipper, Kbool
        Dim inputRegion As Region = CopyRegion(input)
        Dim adaptedPolygonSet As New PolygonSet
        For Each polygonSet As PolygonSet In inputRegion.PolygonSets
            For Each polygon As Polygon In polygonSet.Polygons
                adaptedPolygonSet.Polygons.Add(polygon)
            Next
            For Each polygon As Polygon In polygonSet.Holes
                adaptedPolygonSet.Polygons.Add(polygon)
            Next
        Next
        Return adaptedPolygonSet
    End Function
End Class

Public Class InputAdapterFromRegionToPolygonSetWithExplicitHoles
    Public Function GetAdaptedInput(ByVal input As Region) As PolygonSet
        'Gpc
        Dim inputRegion As Region = CopyRegion(input)
        Dim adaptedPolygonSet As New PolygonSet
        For Each polygonSet As PolygonSet In inputRegion.PolygonSets
            For Each polygon As Polygon In polygonSet.Polygons
                adaptedPolygonSet.Polygons.Add(polygon)
            Next
            For Each polygon As Polygon In polygonSet.Holes
                adaptedPolygonSet.Holes.Add(polygon)
            Next
        Next
        Return adaptedPolygonSet
    End Function
End Class

Public Class InputAdapterFromRegionToRegionWithClockwiseOuter
    Public Function GetAdaptedInput(ByVal input As Region) As Region
        'Bop, Geos, Ggl, TerraLib
        Dim inputRegion As Region = CopyRegion(input)
        Return inputRegion
    End Function
End Class

Public Class InputAdapterFromRegionToRegionWithAnticlockwiseOuter
    Public Function GetAdaptedInput(ByVal input As Region) As Region
        'Cgal, Gtl, PolyBoolean, MSSqlServerTypes
        Dim inputRegion As Region = CopyRegion(input)
        ReverseRegion(inputRegion)
        Return inputRegion
    End Function
End Class


Public Class OutputAdapterFromPolygonSetToPolygonSet
    Public Function GetAdaptedOutput(ByVal output As PolygonSet
            ) As PolygonSet
        Return CopyPolygonSet(output)
    End Function
End Class

Public Class OutputAdapterFromRegionToPolygonSet
    Public Function GetAdaptedOutput(ByVal output As Region)
        Dim outputRegion As Region = CopyRegion(output)
        Dim adaptedPolygonSet As New PolygonSet
        For Each polygonSet As PolygonSet In outputRegion.PolygonSets
            For Each polygon As Polygon In polygonSet.Polygons
                adaptedPolygonSet.Polygons.Add(polygon)
            Next
            For Each polygon As Polygon In polygonSet.Holes
                adaptedPolygonSet.Polygons.Add(polygon)
            Next
        Next
        Return adaptedPolygonSet
    End Function
End Class