﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Domain;

namespace ConfigurationWindow
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SearchEntry> SearchEntries = new List<SearchEntry>();

        private readonly ITagDataBase tagDb = new TagDataBase();

        private delegate void SetFields(TagData tagData);

        public MainWindow()
        {
            InitializeComponent();

            // Set up connection to database
            tagDb.Connect();

            // Set up reader communicator
            var reader = ReaderCommunicator.GetInstance();
            reader.Connect();
            reader.ActivateScan();
            reader.NewTagScanned += ReaderOnNewTagScanned;
        }

        private void ReaderOnNewTagScanned(TagData tagData)
        {
            // Tell UI-Thread to fill fields
            textBoxArticleName.Dispatcher.BeginInvoke(new SetFields(FieldSetter), tagData);
        }

        public void FieldSetter(TagData tagData)
        {
            // Set the fields
            textBoxSearch.Text = tagData.Id;
            textBoxChipNumber.Text = tagData.Id;
            textBoxChipTimestamp.Text = tagData.TimeStamp.ToString(CultureInfo.InvariantCulture);
            textBoxChipData.Text = tagData.Data;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            bool tagsFound = false;
            bool articleFound = false;

            // Check for db conntection
            if (!tagDb.IsConnected())
                return;

            // Clear previous search entries
            SearchEntries.Clear();

            // Check which fields are set
            if (textBoxSearch.Text != "")
            {
                int parsedNumber = 0;

                // Try parse given number
                if (!Int32.TryParse(textBoxSearch.Text, out parsedNumber))
                {
                    labelStatus.Content = "Status: Bitte gültige Nummer eingeben!";
                    return;
                }

                // Search by chip number
                ArticleData articleFromDb = tagDb.GetArticleDataByTagData(parsedNumber);

                // Check return value
                if (articleFromDb != null)
                {
                    articleFound = true;

                    SearchEntries.Add(new SearchEntry(articleFromDb.Id.ToString(), articleFromDb.Name, articleFromDb.Note, articleFromDb.Cost,
                        "", DateTime.Now, ""));

                    // TODO Refresh search entries? needed?
                }

                // Search by article number
                List<TagData> tagsFromDb = tagDb.GetTagDataByArticleData(parsedNumber);

                // Check return value
                if (tagsFromDb.Count >= 1)
                {
                    tagsFound = true;

                    foreach (var td in tagsFromDb)
                    {
                        SearchEntries.Add(new SearchEntry("", "", "", 0.00M, td.Id, td.TimeStamp, td.Data));
                    }

                    // TODO Refresh search entries? needed?
                }

                // Set status label
                if (articleFound)
                {
                    if (tagsFound)
                    {
                        labelStatus.Content = "Status: Artikel und Tags gefunden.";
                    }
                    else
                    {
                        labelStatus.Content = "Status: Artikel gefunden.";
                    }
                }
                else
                {
                    if (tagsFound)
                    {
                        labelStatus.Content = "Status: Tags gefunden.";
                    }
                    else
                    {
                        labelStatus.Content = "Status: Suche ergab keine Ergebnisse.";
                    }
                }

            }
        }

        private void buttonDeleteLink_Click(object sender, RoutedEventArgs e)
        {
            // Check for db connection
            if (!tagDb.IsConnected())
                return;

            // Check if field is set
            if (textBoxChipNumber.Text != "")
            {
                // Delete the entry with the given chip number
                if (tagDb.DeleteLink(new TagData(textBoxChipNumber.Text, DateTime.Now, "")))
                {
                    // Tag could be deleted
                    labelStatus.Content = "Status: Link erfolgreich gelöscht.";
                }
                else
                {
                    labelStatus.Content = "Status: Link konnte nicht gelöscht werden";
                }
            }
        }

        private void buttonAddLink_Click(object sender, RoutedEventArgs e)
        {
            TagData tagData = new TagData(textBoxChipNumber.Text, DateTime.Parse(textBoxChipTimestamp.Text), textBoxChipData.Text);
            ArticleData articleData = new ArticleData();
            articleData.Id = Convert.ToInt32(textBoxArticleNumber.Text);
            articleData.Name = textBoxArticleName.Text;
            articleData.Cost = Convert.ToDecimal(textBoxArticlePrice.Text);
            articleData.Note = textBoxArticleNote.Text;

            if (tagDb.IsConnected())
            {
                if (tagDb.CreateLink(tagData, articleData))
                {
                    labelStatus.Content = "Status: Link erfolgreich hinzugefügt.";
                }
                else
                {
                    labelStatus.Content = "Status: Link konnte nicht hinzugefügt werden!";
                }
            }
        }
    }

    public class SearchEntry
    {
        public decimal ArticleCost;
        public string ArticleData;
        public string ArticleName;
        public string ArticleNote;
        public string ArticleNumber;
        public DateTime ArticleTimestamp;
        public string TagId;

        public SearchEntry(string articleNumber, string articleName, string articleNote, decimal articleCost,
            string tagId, DateTime tagTimestamp, string tagData)
        {
            ArticleNumber = articleNumber;
            ArticleName = articleName;
            ArticleNote = articleNote;
            ArticleCost = articleCost;
            TagId = tagId;
            ArticleTimestamp = tagTimestamp;
            ArticleData = tagData;
        }
    }
}