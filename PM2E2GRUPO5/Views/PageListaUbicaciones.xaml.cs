﻿using PM2E2GRUPO5.Controllers;
using PM2E2GRUPO5.Models;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace PM2E2GRUPO5.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageListaUbicaciones : ContentPage
    {
        ItemTappedEventArgs elemento = null;
        public PageListaUbicaciones()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            recargar();
        }
        private async void recargar()
        {
            lblelementos.Text = "cargando...";
            ListaSitios.ItemsSource = null;
            elemento = null;
            btnactualizar.IsEnabled = false;
            btnreproduciraudio.IsEnabled = false;
            btnvermapa.IsEnabled = false;
            ListaSitios.ItemsSource = await SitioController.GetAllSite();

            int c = 0;
            foreach (var item in ListaSitios.ItemsSource)
            {
                c++;
            }
            lblelementos.Text = "Registros ";
        }

        private async void nuevaubicacion_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());

        }

        private void ListaSitios_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            elemento = e;
            btnactualizar.IsEnabled = true;
            btnreproduciraudio.IsEnabled = true;
            btnvermapa.IsEnabled = true;

        }

        private void btnactualizar_Clicked(object sender, EventArgs e)
        {

            var sitio = (SitiosFirma)elemento.Item;
            Navigation.PushAsync(new PageModificar(sitio));

        }

        private async void btnvermapa_Clicked(object sender, EventArgs e)
        {
            var sitio = (SitiosFirma)elemento.Item;
            await Navigation.PushAsync(new PageMapa(Double.Parse(sitio.Latitud), Double.Parse(sitio.Longitud), sitio.Descripcion)); // Abre el Page de Mapa


        }

        private void btnreproduciraudio_Clicked(object sender, EventArgs e)
        {
            var sitio = (SitiosFirma)elemento.Item;

            // Crea un archivo temporal y obtiene 
            // su ruta:
            string archivoTemp = Path.GetTempFileName();
            File.WriteAllBytes(archivoTemp, sitio.AudioFile);

            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.Play(archivoTemp);

            File.Delete(archivoTemp);

        }
    }
}