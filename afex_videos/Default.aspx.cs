using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace afex_videos
{
    public partial class _Default : Page
    {
        /// <summary>
        /// Cargar la página 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (Request.Params.Get("__EVENTARGUMENT") != null && Request.Params.Get("__EVENTARGUMENT") != "")
                {
                    EliminarVideo(Request.Params.Get("__EVENTARGUMENT"));
                }
                else
                {
                    string cVideo = Request.Form["txtVideo"];

                    InsertarVideo(cVideo);
                }
            }

            CargarGaleria();
        }

        /// <summary>
        /// Elimina un video de la galería
        /// </summary>
        /// <param name="IdVideo">Identificador del video</param>
        private void EliminarVideo(string IdVideo)
        {
            using (var data = new AFEXGaleriaVideosEntities())
            {
                var oDelete = data.videos.Find(Convert.ToInt32(IdVideo));

                if (oDelete != null)
                {
                    try
                    {
                        data.videos.Remove(oDelete);
                        data.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ViewError(string.Format("Error al eliminar el video. ERROR:{0}", ex.Message));
                    }
                }
            }
        }


        /// <summary>
        /// Insertar video en la base de datos
        /// </summary>
        /// <param name="URLVideo">URL del video a insertar</param>
        private void InsertarVideo(string URLVideo)
        {
            string cErrorMSG = @"La URL no es válida, asegúrese de que el formato de la URL sea del tipo &quot;https://www.youtube.com/watch?v=[ID DEL VIDEO]&quot;";

            lblError.Text = "";
            lblError.Visible = false;

            if (URLVideo.Trim() == string.Empty)
            {
                ViewError(cErrorMSG);
                return;
            }

            Uri cURLProcesada;

            // Validar que el URL sea válido
            if (Uri.TryCreate(URLVideo, UriKind.Absolute, out cURLProcesada))
            {
                if (cURLProcesada.AbsoluteUri.ToLower().IndexOf("https://www.youtube.com/watch?v=") == -1)
                {
                    ViewError(cErrorMSG);
                    return;
                }

                try
                {
                    // Guardar URL en la base de datos
                    using (var data = new AFEXGaleriaVideosEntities())
                    {
                        videos oNew = new videos();

                        oNew.video = cURLProcesada.AbsoluteUri;
                        oNew.fecha = DateTime.Now;

                        data.videos.Add(oNew);

                        data.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ViewError(string.Format("Error al guardar, ERROR:{0}", ex.Message));
                }
            }
            else
            {
                ViewError(cErrorMSG);
            }
        }

        /// <summary>
        /// Muestra un mensaje de error en formato de alerta
        /// </summary>
        /// <param name="cErrorMSG">Mensaje a mostrar</param>
        private void ViewError(string cErrorMSG)
        {
            lblError.Text = cErrorMSG;
            lblError.Visible = true;
        }


        /// <summary>
        /// Cargar la lista de videos de la base de datos y la visualiza en pantalla
        /// </summary>
        private void CargarGaleria()
        {
            int nContar = 0;

            using (var data = new AFEXGaleriaVideosEntities())
            {
                // Obtiene la lista de videos de la base de datos
                var oVideos = (from v in data.videos
                               orderby v.Id descending
                               select new
                               {
                                   v.Id,
                                   v.video
                               }).ToList();

                // Generar galería
                for (int i = 0; i < oVideos.Count; i++)
                {
                    if (nContar == 0)
                    {
                        pnlGaleria.Controls.Add(new LiteralControl("<div class='row'>"));
                    }

                    string cIdVideoEmbeded = oVideos[i].video;
                    cIdVideoEmbeded = cIdVideoEmbeded.Substring(cIdVideoEmbeded.ToLower().IndexOf("?v=") + 3, 11);

                    string cVideoImagen = oVideos[i].video.Replace("watch?v=", "vi/").Replace("www.youtube", "img.youtube")+"/0.jpg";

                    // Insertar la miniatura
                    string cHTMLCode = string.Format(@"<div class='col-lg-3'>
                                                            <input type='button' class='btn btn-dark btn-sm float-right' value='X' style='position:absolute !important; left:245px; top:3px;' value='X' title='Eliminar' name='btnEliminar_{2}' onclick='Eliminar({2})'/>
                                                            <img src='{0}' width='100%' style='cursor: pointer;' onclick='VerEnPopup(&quot;{0}|{1}&quot;)'></img>
                                                        </div>", cVideoImagen, cIdVideoEmbeded, oVideos[i].Id);


                    pnlGaleria.Controls.Add(new LiteralControl(cHTMLCode));

                    nContar++;

                    if (nContar == 4)
                    {
                        pnlGaleria.Controls.Add(new LiteralControl("</div><br>"));
                        nContar = 0;
                    }
                }
            }
        }
    }
}