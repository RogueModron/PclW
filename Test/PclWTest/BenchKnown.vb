Imports PclWCommon
Imports PclWTestCommon

Module BenchKnown
    Public Sub TestSpeedKnown(ByVal testObj As TestableObj,
                ByVal intersections As Integer, ByVal loops As Integer,
                ByVal angleOffset As Double)
        Dim intersections4 As Integer = Fix(intersections / 4)
        Dim rays As Integer = Math.Floor(Math.Sqrt(intersections4))
        Dim cilinders As Integer = rays
        If (intersections4 - rays * cilinders) > rays Then
            rays = rays + 1
        End If
        Dim longerRays As Integer = intersections4 - rays * cilinders
        If longerRays = rays Then
            rays = rays + 1
            longerRays = 0
        End If

        Const radiusStep As Double = 2.6
        Dim internalRadius As Integer = radiusStep * rays * Math.PI * 2
        Dim raysRadius As Double = internalRadius + cilinders * radiusStep +
            radiusStep
        Dim longerRaysRadius As Double = raysRadius + radiusStep * 2

        Dim psA As New PolygonSet
        psA.Polygons.Add(GetTestClockwiseGearPolygon(rays, longerRays,
            raysRadius, longerRaysRadius, internalRadius))
        Dim oA As Object = testObj.GetAdaptedInputFromPolygonSet(psA)

        Dim psB As New PolygonSet
        If longerRays > 0 Then
            cilinders = cilinders + 1
        End If
        For counter As Integer = 1 To cilinders
            psB.Polygons.Add(GetTestClockwisePolygon(rays * 2,
                internalRadius + counter * radiusStep + radiusStep / 2, 0))
            psB.Polygons.Add(GetTestClockwisePolygon(rays * 2,
                internalRadius + counter * radiusStep, 0))
            ReverseLastPolygon(psB)
        Next
        Dim oB As Object = testObj.GetAdaptedInputFromPolygonSet(psB)

        Dim oC As Object = Nothing

        Dim watch As New Stopwatch
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            'Threading.Thread.Sleep(1000)
            watch.Start()
            For k As Integer = 1 To loops
                oC = testObj.GetIntersection(oA, oB)
            Next
            watch.Stop()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return
        End Try
        Dim psC As PolygonSet = testObj.GetAdaptedOutputToPolygonSet(oC)

        'Dim svgWriter As New SvgWriter
        'svgWriter.AdaptViewBox = True
        'svgWriter.StrokeWidth = 3 * (Fix(longerRaysRadius / 600) + 1)
        'svgWriter.Write(psA, testObj.Id & ".KNAA" & intersections.ToString("00"))
        'svgWriter.Write(psB, testObj.Id & ".KNAB" & intersections.ToString("00"))
        'svgWriter.Write(psC, testObj.Id & ".KNAC" & intersections.ToString("00"))

        Dim sourcePolygonsCount As Integer = GetPolygonsCount(psA) +
            GetPolygonsCount(psB)
        Dim sourceVerticesCount As Integer = GetVerticesCount(psA) +
            GetVerticesCount(psB)
        Dim resultPolygonsCount As Integer = GetPolygonsCount(psC)
        Dim resultVerticesCount As Integer = GetVerticesCount(psC)
        Dim message As String =
            String.Format("TK {0,-10} ", testObj.Id) &
            String.Format("l {0:0000} ", loops) &
            String.Format("ms {0:00000000} ", watch.ElapsedMilliseconds) &
            String.Format("PS {0:00000} ", sourcePolygonsCount) &
            String.Format("VS {0:00000000} ", sourceVerticesCount) &
            String.Format("PR {0:000000} ", resultPolygonsCount) &
            String.Format("VR {0:00000000}", resultVerticesCount)
        Console.WriteLine(message)
    End Sub
 End Module
