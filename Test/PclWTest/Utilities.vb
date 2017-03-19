Imports System.Collections.ObjectModel
Imports System.Math
Imports PclWCommon

Module Utilities
    Public Function CalculateArea(ByVal polygon As Polygon) As Double
        Dim vertices As Collection(Of Vertex) = polygon.Vertices

        If polygon.Vertices.Count < 3 Then
            Return 0
        End If

        Dim vertexA = vertices(vertices.Count - 1)
        Dim vertexB = vertices(0)
        Dim area As Double = (vertexA.Y * vertexB.X) - (vertexA.X * vertexB.Y)
        For i As Integer = 0 To vertices.Count - 2
            vertexA = vertices(i)
            vertexB = vertices(i + 1)
            area += (vertexA.Y * vertexB.X) - (vertexA.X * vertexB.Y)
        Next

        Return (area / 2)
    End Function

    Public Function GetRandomDouble() As Double
        Static generator As New Random
        Return (generator.NextDouble - 0.5) / 0.5
    End Function

    Public Function GetTestClockwiseGearPolygon(
            ByVal rays As Integer, ByVal longerRays As Integer,
            ByVal raysRadius As Double, ByVal longerRaysRadius As Double,
            ByVal internalRadius As Double,
            Optional ByVal angleOffset As Double = 0) As Polygon
        Dim p As New Polygon

        Dim rotationStep As Double = -PI / rays
        For rayIndex As Integer = 0 To rays - 1
            Dim angle As Double = rotationStep * 2 * rayIndex +
                angleOffset
            Dim x As Double = internalRadius * Cos(angle)
            Dim y As Double = internalRadius * Sin(angle)
            p.Vertices.Add(New Vertex(x, y))

            Dim externalRadius As Double = raysRadius
            If longerRays > 0 Then
                externalRadius = longerRaysRadius
                longerRays = longerRays - 1
            End If
            x = externalRadius * Cos(angle)
            y = externalRadius * Sin(angle)
            p.Vertices.Add(New Vertex(x, y))

            angle = angle + rotationStep
            x = externalRadius * Cos(angle)
            y = externalRadius * Sin(angle)
            p.Vertices.Add(New Vertex(x, y))

            x = internalRadius * Cos(angle)
            y = internalRadius * Sin(angle)
            p.Vertices.Add(New Vertex(x, y))
        Next

        Return p
    End Function

    Public Function GetTestClockwisePolygon(
            ByVal externalSidesCount As Integer,
            ByVal externalRadius As Double, ByVal internalRadius As Double,
            Optional ByVal angleOffset As Double = 0,
            Optional ByVal xOffset As Double = 0,
            Optional ByVal yOffset As Double = 0) As Polygon
        Dim p As New Polygon

        Dim addExternalFirst As Boolean = False
        Dim isGear As Boolean = (internalRadius <> 0)
        Dim rotationStep As Double = -PI * 2 / externalSidesCount
        For sideIndex As Integer = 0 To externalSidesCount
            Dim angle As Double = rotationStep * sideIndex + angleOffset
            Dim xExternal As Double = externalRadius * Cos(angle) + xOffset
            Dim yExternal As Double = externalRadius * Sin(angle) + yOffset
            If isGear Then
                If (sideIndex = externalSidesCount AndAlso addExternalFirst) _
                        OrElse sideIndex = 0 Then
                    p.Vertices.Add(New Vertex(xExternal, yExternal))
                Else
                    Dim xInternal As Double = internalRadius * Cos(angle) +
                        xOffset
                    Dim yInternal As Double = internalRadius * Sin(angle) +
                        yOffset
                    If addExternalFirst Then
                        p.Vertices.Add(New Vertex(xExternal, yExternal))
                        p.Vertices.Add(New Vertex(xInternal, yInternal))
                    Else
                        p.Vertices.Add(New Vertex(xInternal, yInternal))
                        p.Vertices.Add(New Vertex(xExternal, yExternal))
                    End If
                End If
                addExternalFirst = Not addExternalFirst
            Else
                p.Vertices.Add(New Vertex(xExternal, yExternal))
            End If
        Next

        Return p
    End Function

    Public Sub ReadMemory()
        Static processName As String = Process.GetCurrentProcess().ProcessName
        Static counter As PerformanceCounter =
            New PerformanceCounter("Process", "Private Bytes", processName)
        Console.WriteLine("xx {0}", counter.RawValue / 1024)
    End Sub
End Module

