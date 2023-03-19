using Nancy.Json;
using Plugin.AudioRecorder;
using PM2E2GRUPO5.Models;
using PM2E2GRUPO5.Controllers;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM2E2GRUPO5
{
    public partial class MainPage : ContentPage
    {

        bool flag2 = false;

        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();

        SitioController sitiosApi;
        List<SitiosFirma> ListaSitios;
        public MainPage()
        {
            InitializeComponent();
            checkInternet();
            getLocation();

            sitiosApi = new SitioController();
            ListaSitios = new List<SitiosFirma>();
            flag2 = false;

        }

        private async void btnUbicaciones_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.PageListaUbicaciones());

        }

        private void Limpiardescripcion_Clicked(object sender, EventArgs e)
        {
            descripcion.Text = null;

        }

        private async void btngrabarvoz_Clicked(object sender, EventArgs e)
        {

            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            var status2 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var status3 = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted && status2 != PermissionStatus.Granted && status3 != PermissionStatus.Granted)
            {
                return; // si no tiene los permisos no avanza
            }

            onda1.IsVisible = true;
            onda2.IsVisible = true;
            ondaespacio.IsVisible = false;
            imgmicro.Source = "voice.png";
            btnGuardar.IsEnabled = false;
            btngrabarvoz.IsVisible = false;
            btndetenervoz.IsVisible = true;

            await audioRecorderService.StartRecording();

            flag2 = true;

        }

        private async void btndetenervoz_Clicked(object sender, EventArgs e)
        {

            onda1.IsVisible = false;
            onda2.IsVisible = false;
            ondaespacio.IsVisible = true;
            ondaespacio.Text = "¡Guardado!";
            imgmicro.Source = "voiceoff.png";
            btnGuardar.IsEnabled = true;
            btngrabarvoz.IsVisible = true;
            btndetenervoz.IsVisible = false;

            await audioRecorderService.StopRecording();

        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {

            bool flag1 = false;
            if (latitud.Text == null || longitud.Text == null)
            {
                flag1 = true;
                await DisplayAlert("Error", "Se necesitan las coordenadas de su ubicación para guardar.", "OK");
            }

            if (descripcion.Text == null || descripcion.Text == "")
            {
                flag1 = true;
                await DisplayAlert("Error", "Se necesita una breve descripción de la ubicación.", "OK");
            }

            if (!flag1)
            {
                byte[] ImageBytes = null;
                byte[] AudioBytes = null;
                var firma = PadView.Strokes;


                //obtenemos la firma
                try
                {
                    var image = await PadView.GetImageStreamAsync(SignatureImageFormat.Png);

                    //Pasamos la firma a imagen a base 64
                    var mStream = (MemoryStream)image;
                    byte[] data = mStream.ToArray();
                    string base64Val = Convert.ToBase64String(data);
                    ImageBytes = Convert.FromBase64String(base64Val);
                }
                catch (Exception error)
                {
                    await DisplayAlert("Aviso", "No has ingresado tu firma", "OK");
                    return;
                }

                //obtenemos el audio
                try
                {
                    var audio = audioRecorderService.GetAudioFileStream();


                    AudioBytes = File.ReadAllBytes(audioRecorderService.GetAudioFilePath());
                }
                catch (Exception error)
                {
                    if (flag2)
                    {
                        await DisplayAlert("Aviso", "No has hablado fuerte al grabar tu nota de voz", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "No has grabado una nota de voz", "OK");
                    }

                    return;
                }

                try
                {
                    byte[] a;
                    var serializer = new JavaScriptSerializer();

                    var b = serializer.Serialize(firma);

                    a = null;

                    SitiosFirma sitio = new SitiosFirma
                    {
                        Descripcion = descripcion.Text,
                        Latitud = latitud.Text,
                        Longitud = longitud.Text,
                        FirmaDigital = ImageBytes,
                        AudioFile = AudioBytes,
                        firma = b
                    };

                    await SitioController.CreateSite(sitio);
                    await DisplayAlert("Aviso", "Sitio ingresado con éxito: " + sitio.Descripcion, "OK");
                    PadView.Clear();
                    descripcion.Text = null;

                }
                catch (Exception error)
                {
                    await DisplayAlert("Aviso", "" + error, "OK");
                }


            }

        }

        #region Location
        private void cleanLocation()
        {
            latitud.Text = null;
            longitud.Text = null;
        }

        public async void getLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    latitud.Text = "" + location.Latitude;
                    longitud.Text = "" + location.Longitude;
                    //await DisplayAlert("Aviso", "Si se leyó la ubicacion: "+location.Latitude +", "+location.Longitude, "OK");
                }
                else
                {
                    await DisplayAlert("Aviso", "El GPS está desactivado o no puede reconocerse", "OK");
                    cleanLocation();
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Aviso", "Este dispositivo no soporta la versión de GPS utilizada", "OK");
                cleanLocation();
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                //await DisplayAlert("Aviso", "Handle not enabled on device exception: "+fneEx, "OK");
                await DisplayAlert("Aviso", "La ubicación está desactivada", "OK");
                cleanLocation();

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Aviso", "La aplicación no puede acceder a su ubicación.\n\n" +
                    "Habilite los permisos de ubicación en los ajustes del dispositivo", "OK");
                cleanLocation();
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Aviso", "No se ha podido obtener la localización (error de gps)", "OK");
                cleanLocation();
            }
        }
        #endregion

        #region Internet
        public async void checkInternet()
        {
            //await DisplayAlert("Aviso", "si", "OK");
            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                // Connection to internet is available
                await DisplayAlert("Aviso", "Usted no tiene acceso a Internet.\n" +
                    "El acceso a Internet es requerido para el buen funcionamiento de la aplicación.", "OK");
            }

        }
        #endregion


    }
}
