Imports PclWCommon
Imports PclWTestCommon

Public Class BenchGrid
    Public Enum ModeType
        None
        IntersectionOff
        IntersectionOn
    End Enum

    Private _PolygonSetA As PolygonSet
    Private _PolygonSetB As PolygonSet

    Public Property HolesForRow As Integer
    Public Property Mode As ModeType
    Public Property SideLengthOuter As Double

    Private Sub SetPolygonSet(ByRef polygonSet As PolygonSet,
            ByVal mode As ModeType)
        Dim halfSideLength As Double = _SideLengthOuter / 2

        Dim coordinates() As Double = {
            -halfSideLength, -halfSideLength,
            -halfSideLength, halfSideLength,
            halfSideLength, halfSideLength,
            halfSideLength, -halfSideLength
            }
        polygonSet = New PolygonSet
        polygonSet.Polygons.Add(New Polygon(coordinates))

        Dim innerStep As Double = _SideLengthOuter / (_HolesForRow * 2 + 1)

        Dim x As Double = -halfSideLength
        Dim y As Double = -halfSideLength
        Dim o As Double = 0
        Select Case mode
            Case ModeType.None
                x = x + innerStep
                o = 0
            Case ModeType.IntersectionOff
                x = x + 0
                o = innerStep * 0.1
            Case ModeType.IntersectionOn
                x = x + innerStep / 2
                o = 0
        End Select
        Do While (x + innerStep) < halfSideLength
            y = -halfSideLength
            Select Case mode
                Case ModeType.None
                    y = y + innerStep
                Case ModeType.IntersectionOff
                    y = y + 0
                Case ModeType.IntersectionOn
                    y = y + innerStep / 2
            End Select
            Do While (y + innerStep) < halfSideLength
                coordinates = New Double() {
                    x + o, y + o,
                    x - o + innerStep, y + o,
                    x - o + innerStep, y - o + innerStep,
                    x + o, y - o + innerStep
                    }
                polygonSet.Polygons.Add(New Polygon(coordinates))
                y = y + innerStep * 2
            Loop
            x = x + innerStep * 2
        Loop
    End Sub

    Private Sub SetPolygonSet(ByRef polygonSet As PolygonSet,
            ByVal avoidIntersections As Boolean,
            ByVal pctIntersections As Integer)
        Dim halfSideLength As Double = _SideLengthOuter / 2

        Dim coordinates() As Double = {
            -halfSideLength, -halfSideLength,
            -halfSideLength, halfSideLength,
            halfSideLength, halfSideLength,
            halfSideLength, -halfSideLength
            }
        polygonSet = New PolygonSet
        polygonSet.Polygons.Add(New Polygon(coordinates))

        Dim innerStep As Double = _SideLengthOuter / _HolesForRow
        Dim holeSide As Double = innerStep * 0.375

        Dim intersectionCounter As Integer =
            (_HolesForRow ^ 2) * pctIntersections / 100

        For xCounter As Integer = 1 To _HolesForRow
            Dim x As Double = -halfSideLength + innerStep * (xCounter - 1)
            For yCounter As Integer = 1 To _HolesForRow
                Dim y As Double = -halfSideLength + innerStep * (yCounter - 1)

                Dim offset As Double = innerStep * 0.1
                If Not avoidIntersections Then
                    If intersectionCounter <= 0 Then
                        offset = offset * 2 + holeSide
                    Else
                        offset = offset + holeSide * 0.5
                    End If
                End If

                coordinates = New Double() {
                    x + offset, y + offset,
                    x + offset + holeSide, y + offset,
                    x + offset + holeSide, y + offset + holeSide,
                    x + offset, y + offset + holeSide
                    }
                polygonSet.Polygons.Add(New Polygon(coordinates))

                intersectionCounter = intersectionCounter - 1
            Next
        Next
    End Sub

    Public Sub SetPolygonSets()
        SetPolygonSet(_PolygonSetA, ModeType.None)
        SetPolygonSet(_PolygonSetB, _Mode)
    End Sub

    Public Sub SetPolygonSets(ByVal pctIntersections As Integer)
        SetPolygonSet(_PolygonSetA, True, 0)
        SetPolygonSet(_PolygonSetB, False, pctIntersections)
    End Sub

    Public Sub TestSpeedGrid(ByVal testObj As TestableObj,
            ByVal loops As Integer)
        Dim oA As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetA)
        Dim oB As Object = testObj.GetAdaptedInputFromPolygonSet(_PolygonSetB)

        Dim oC As Object = Nothing

        Dim watch As New Stopwatch
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            'Threading.Thread.Sleep(1000)
            watch.Start()
            For i As Integer = 1 To loops
                oC = testObj.GetIntersection(oA, oB)
            Next
            watch.Stop()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            If TypeOf oA Is Region Then
                oC = New Region
            Else
                oC = New PolygonSet
            End If
        End Try

        Dim psA As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oA)
        Dim psB As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oB)
        Dim psC As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oC)

        'Dim svgWriter As New SvgWriter
        'svgWriter.AdaptViewBox = True
        'Dim viewPort As SvgWriter.ViewportDefinition
        'viewPort.HeightCm = 20
        'viewPort.WidthCm = 20
        'svgWriter.Viewport = viewPort
        'svgWriter.StrokeWidth = _SideLengthOuter / 300
        'svgWriter.Write(psA, testObj.Id & ".GRDA")
        'svgWriter.Write(psB, testObj.Id & ".GRDB")
        'svgWriter.Write(psC, testObj.Id & ".GRDC")

        Dim sourcePolygonsCount As Integer = GetPolygonsCount(psA) +
            GetPolygonsCount(psB)
        Dim sourceVerticesCount As Integer = GetVerticesCount(psA) +
            GetVerticesCount(psB)
        Dim resultPolygonsCount As Integer = GetPolygonsCount(psC)
        Dim resultVerticesCount As Integer = GetVerticesCount(psC)
        Dim message As String =
            String.Format("TG {0,-10} ", testObj.Id) &
            String.Format("l {0:0000} ", loops) &
            String.Format("ms {0:00000000} ", watch.ElapsedMilliseconds) &
            String.Format("PS {0:00000} ", sourcePolygonsCount) &
            String.Format("VS {0:00000000} ", sourceVerticesCount) &
            String.Format("PR {0:000000} ", resultPolygonsCount) &
            String.Format("VR {0:00000000}", resultVerticesCount)
        Console.WriteLine(message)
    End Sub
End Class
