Imports System.Globalization
Imports System.IO
Imports PclWCommon

Public Class SvgWriter
    Private Const DefaultStrokeWidth As Integer = 3
    Private Const DefaultViewBoxSide As Integer = 700
    Private Const DefaultViewportSide As Byte = 7

    Public Structure ViewBoxDefinition
        Property MinX As Integer
        Property MinY As Integer
        Property Height As Integer
        Property Width As Integer
    End Structure

    Public Structure ViewportDefinition
        Property HeightCm As Byte
        Property WidthCm As Byte
    End Structure

    Public Property AdaptViewBox As Boolean
    Public Property ClosePolygons As Boolean
    Public Property DestinationPath As String
    Public Property StrokeWidth As Integer
    Public Property ViewBox As ViewBoxDefinition
    Public Property Viewport As ViewportDefinition


    Public Sub New()
        _AdaptViewBox = False
        _ClosePolygons = True
        _DestinationPath = My.Computer.FileSystem.SpecialDirectories.Temp

        _StrokeWidth = 3

        With _ViewBox
            .MinX = -DefaultViewBoxSide / 2
            .MinY = -DefaultViewBoxSide / 2
            .Height = DefaultViewBoxSide
            .Width = DefaultViewBoxSide
        End With

        With _Viewport
            .HeightCm = DefaultViewportSide
            .WidthCm = DefaultViewportSide
        End With
    End Sub


    Private Sub AppendEnd(ByVal output As Text.StringBuilder)
        output.Append("</svg>")
    End Sub

    Private Sub AppendHeader(ByVal output As Text.StringBuilder)
        With output
            .Append("<?xml version=""1.0"" standalone=""no""?>")
            .Append(vbNewLine)

            .Append("<!DOCTYPE svg PUBLIC ""-//W3C//DTD SVG 1.1//EN"" ")
            .Append("""http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"">")
            .Append(vbNewLine)

            .Append("<svg width=""")
            .Append(_Viewport.WidthCm)
            .Append("cm"" ")
            .Append("height=""")
            .Append(_Viewport.HeightCm)
            .Append("cm"" ")
            .Append("viewBox=""")
            .Append(_ViewBox.MinX)
            .Append(" ")
            .Append(_ViewBox.MinY)
            .Append(" ")
            .Append(_ViewBox.Width)
            .Append(" ")
            .Append(_ViewBox.Height)
            .Append(""" ")
            .Append("xmlns=""http://www.w3.org/2000/svg"" version=""1.1"">")
            .Append(vbNewLine)
        End With
    End Sub

    Private Sub AppendPolygon(ByVal output As Text.StringBuilder,
            ByVal polygon As Polygon)
        If polygon.Vertices.Count = 0 Then
            Return
        End If
        With output
            Dim color As String = ""
            If polygon.IsClockwise Then
                color = "green"
            Else
                color = "red"
            End If

            .Append("<polyline fill=""none"" stroke=""")
            .Append(color)
            .Append(""" stroke-width=""")
            .Append(_StrokeWidth)
            .Append(""" points=""")

            For Each vertexItem As Vertex In polygon.Vertices
                AppendVertex(output, vertexItem)
            Next
            If _ClosePolygons Then
                AppendVertex(output, polygon.Vertices(0))
            End If

            .Append(""" />")
            .Append(vbNewLine)
        End With
    End Sub

    Private Sub AppendVertex(ByVal output As Text.StringBuilder,
            ByVal vertex As Vertex)
        Dim formatProvider As IFormatProvider =
            NumberFormatInfo.InvariantInfo
        With output
            .Append(vertex.X.ToString(formatProvider))
            .Append(",")
            .Append(vertex.Y.ToString(formatProvider))
            .Append(" ")
        End With
    End Sub

    Private Function GetSvgText(ByVal polygonSet As PolygonSet) As String
        Dim output As New Text.StringBuilder(2048)
        AppendHeader(output)
        For Each polygon As Polygon In polygonSet.Polygons
            AppendPolygon(output, polygon)
        Next
        For Each polygon As Polygon In polygonSet.Holes
            AppendPolygon(output, polygon)
        Next
        AppendEnd(output)
        Return output.ToString
    End Function

    Private Sub SetViewBox(ByVal polygonSet As PolygonSet)
        If Not _AdaptViewBox Then
            Return
        End If

        Dim maxX As Double = Double.MinValue
        Dim maxY As Double = Double.MinValue
        Dim minX As Double = Double.MaxValue
        Dim minY As Double = Double.MaxValue

        For Each polygon As Polygon In polygonSet.Polygons
            For Each vertex As Vertex In polygon.Vertices
                If vertex.X > maxX Then
                    maxX = vertex.X
                End If
                If vertex.X < minX Then
                    minX = vertex.X
                End If
                If vertex.Y > maxY Then
                    maxY = vertex.Y
                End If
                If vertex.Y < minY Then
                    minY = vertex.Y
                End If
            Next
        Next

        Dim height As Double = maxY - minY
        Dim width As Double = maxX - minX
        If height < DefaultViewBoxSide OrElse width < DefaultViewBoxSide Then
            Return
        End If
        With _ViewBox
            .MinX = -width / 2
            .MinY = -height / 2
            .Height = height
            .Width = width
        End With
    End Sub

    Public Sub Write(ByVal polygonSet As PolygonSet,
            ByVal fileName As String)
        Try
            SetViewBox(polygonSet)

            Dim svgText As String = GetSvgText(polygonSet)
            Dim fullFileName As String = Path.Combine(
                _DestinationPath, fileName & ".svg")
            File.WriteAllText(fullFileName, svgText.ToString)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
