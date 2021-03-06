﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RFactor2VREcranSwitcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();

            List<string> dossiers = new List<string>();
            List<string> fichiersPath = new List<string>();
            List<Fichier> fichiers = null;


            // Chemin du fichier de configuration
            string path = Environment.CurrentDirectory;

            Mode mode = Mode.None;

            try
            {
                dossiers.Add(Path.Combine(path, @"UserData\"));
                dossiers.Add(Path.Combine(path, @"UserData\player\"));

                fichiers = GetFichiers(dossiers, fichiersPath);

                foreach (Fichier fichier in fichiers)
                {
                    string fichierEcran = fichier.Path.Replace("." + fichier.Extension.ToString(), ".ecran");
                    string fichierVR = fichier.Path.Replace("." + fichier.Extension.ToString(), ".vr");

                    // Si on a qu'un fichier, on va le dupliquer en .VR pour faciliter la vie de l'utilisateur.
                    if (!File.Exists(fichierVR) && !File.Exists(fichierEcran))
                    {
                        File.Copy(fichier.Path, fichierVR);
                        // si fichier config graphique
                        if (fichier.Path.EndsWith("Config_DX11.ini"))
                        {
                            // On désactive la VR dans le fichier d'origine
                            this.DesactiverVRDansConfig_DX11Ini(fichier.Path);

                            // On active la VR dans le fichier créé
                            this.ActiverVRDansConfig_DX11Ini(fichierVR);

                            //MessageBox.Show("rendererDX11.ini dupliqué en rendererDX11.VR");
                        }
                    }// End if premier lancement


                    // Si on a un .ini et un .VR, on passe en mode VR
                    if (File.Exists(fichierVR) && !File.Exists(fichierEcran))
                    {
                        File.Move(fichier.Path, fichierEcran);
                        File.Move(fichierVR, fichier.Path);
                        mode = Mode.VR;
                    }
                    // Si on a un .ini et un .Ecran, on passe en mode Ecran
                    else if (!File.Exists(fichierVR) && File.Exists(fichierEcran))
                    {
                        File.Move(fichier.Path, fichierVR);
                        File.Move(fichierEcran, fichier.Path);
                        mode = Mode.Ecran;
                    }
                    // Si on a un .Ecran et un .jeu, soit il y'a un bug soit c'est l'utilisateur le bug
                    else if (File.Exists(fichierVR) && File.Exists(fichierEcran))
                    {
                        throw new Exception("Vous ne devez pas avoir un " + fichierVR + " ET un " + fichierEcran + '.');
                    }
                }

                MessageBox.Show("Nouveau mode: " + mode);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur." + e);
            }

            // On quitte l'appli
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }

        private static List<Fichier> GetFichiers(List<string> dossiers, List<string> fichiersPath)
        {
            List<Fichier> fichiers = new List<Fichier>();

            foreach (string dossier in dossiers)
            {
                fichiersPath = new List<string>();
                fichiersPath.AddRange(Directory.GetFiles(dossier, "*.ini"));
                fichiersPath.AddRange(Directory.GetFiles(dossier, "*.JSON"));

                foreach (string fichierPath in fichiersPath)
                {
                    if (fichierPath.EndsWith(".json"))
                        fichiers.Add(new Fichier(fichierPath, Extension.json));

                    if (fichierPath.EndsWith(".JSON"))
                        fichiers.Add(new Fichier(fichierPath, Extension.JSON));

                    if (fichierPath.EndsWith(".ini"))
                        fichiers.Add(new Fichier(fichierPath, Extension.ini));
                }
            }

            return fichiers;
        }

        /// <summary>
        /// Desactive la VR dans le fichier Render.ini.
        /// </summary>
        /// <param name="path">Le chemin complet du fichier.</param>
        private void DesactiverVRDansConfig_DX11Ini(string path)
        {
            try
            {
                var lignesRendererDX11 = File.ReadAllLines(path);

                for (int i = 0; i < lignesRendererDX11.Count(); i++)
                {
                    if (lignesRendererDX11[i].Length >= 11 && lignesRendererDX11[i].StartsWith("VrSettings"))
                    {
                        lignesRendererDX11[i] = lignesRendererDX11[i].Replace("1", "0");
                    }
                }

                File.WriteAllLines(path, lignesRendererDX11);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                throw new Exception("Erreur." + e);
            }
        }

        /// <summary>
        /// Active la VR dans le fichier Render.ini.
        /// </summary>
        /// <param name="path">Le chemin complet du fichier.</param>
        private void ActiverVRDansConfig_DX11Ini(string path)
        {
            try
            {
                var lignesRendererDX11 = File.ReadAllLines(path);

                for (int i = 0; i < lignesRendererDX11.Count(); i++)
                {
                    if (lignesRendererDX11[i].Length >= 11 && lignesRendererDX11[i].StartsWith("VrSettings"))
                    {
                        lignesRendererDX11[i] = lignesRendererDX11[i].Replace("0", "1");
                    }
                }
                File.WriteAllLines(path, lignesRendererDX11);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Fichier non trouvé dans " + path);
            }
            catch (Exception e)
            {
                throw new Exception("Erreur." + e);
            }
        }
    }

    /// <summary>
    /// Liste les périphériques utilisables.
    /// </summary>
    public enum Mode
    {
        None = 0,

        /// <summary>
        /// Oculus Rift ou OpenVR.
        /// </summary>
        VR = 1,

        /// <summary>
        /// Écran.
        /// </summary>
        Ecran = 2
    }
}
