<%@ Page Title="Principal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="afex_videos._Default" %>

<asp:Content ID="HeaderCnt" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>

    <script>
        // Envia el video a la ventana de reproducción
        function Reproducir() {
            var IdVideo = document.getElementById("hidenVideoReproducir");

            window.open("Reproducir.aspx?id=" + IdVideo.value, '_self');
        }

        // Visualiza la imagen, el título y la descripción del video seleccionado en un popup para su posterior visualización si así se desea
        function VerEnPopup(urlImagen) {
            var aR = urlImagen.split("|");
            $("#imgVideoPopup").attr("src", aR[0]);
            $("#hidenVideoReproducir").attr("value", aR[1]);
            $("#popupPropiedades").modal("show");

            $.ajax({
                type: 'GET',
                url: 'https://www.googleapis.com/youtube/v3/videos?id=' + aR[1] + '&key=AIzaSyAXHG6LYuvw9rxlLGV2N7K-H-3JnGSnybY&part=snippet',
                dataType: 'json',
                success: function (data) {
                    AplicarTituloYDescripcion(data)
                }
            });
        }

        // Visualiza el título y la descripción del video en el popup
        function AplicarTituloYDescripcion(data) {
            if (data != null) {
                var cTD = "<h4>"+data.items[0].snippet.title+"</h4>";
                cTD += "<br>" + data.items[0].snippet.description;
                document.getElementById('<%=lblCaracteristicas.ClientID%>').innerHTML = cTD;
            }
        }

        // Elimina el video seleccionado
        function Eliminar(IdVideo) {
            if (confirm("¿Está seguro de eliminar el video?")) {
                __doPostBack('IdEliminar', IdVideo);
            }
        }

        // Envía un parámetro via postback 
        function __doPostBack(eventTarget, eventArgument) {
            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.__EVENTTARGET.value = eventTarget;
                theForm.__EVENTARGUMENT.value = eventArgument;
                theForm.submit();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Añadir nuevo video</h1>
    <asp:UpdatePanel ID="pnlPrincipal" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="padding-bottom: 20px;">
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 90%">
                            <input type="text" class="form-control" name="txtVideo">
                        </td>
                        <td>
                            <input type="submit" name="btnAddVideo" class="btn btn-primary w-100" value="Añadir" />
                        </td>
                    </tr>
                </table>
                <asp:Label runat="server" ID="lblError" CssClass="alert-danger" Width="100%" Visible="false"></asp:Label>
            </div>

            <asp:Panel ID="pnlGaleria" runat="server">
            </asp:Panel>

            <div class="modal fade" id="popupPropiedades" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <input type="hidden" id="hidenVideoReproducir" />
                            <div class="row">
                                <div class="col-5">
                                    <img id="imgVideoPopup" alt="" style="width: 270px" />
                                </div>
                                
                                <div class="col-7 pre-scrollable" style="height:280px !important;">
                                    <asp:Label runat="server" ID="lblCaracteristicas"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" name="btnReproducir" class="btn btn-primary" onclick="Reproducir();">
                                <i class="fa fa-play-circle"></i>&nbsp;Reproducir
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
