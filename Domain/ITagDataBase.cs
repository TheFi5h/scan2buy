﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    interface ITagDataBase
    {
        void SetUpDataBase();                                           // to create the needed tables when first using the database
        void Connect();                                                 // to start the db or initate a link to it maybe add info here
        void Disconnect();                                              // to close the connection to the db

        bool CreateLink(TagData tagData, ArticleData articleData);      // to link TagData to a specific article
        bool DeleteLink(TagData tagData);                               // to delete the set link by the tag id
        bool DeleteLinks(ArticleData articleData);                      // to delete the set link by the article id

        ArticleData GetArticleDataByTagData(TagData tagData);           // to get the ArticleData linked to the given TagData
        ArticleData GetArticleDataByTagData(int id);                    // to get the ArticleData linked to the given TagData

        List<TagData> GetTagDataByArticleData(ArticleData articleData); // to get the TagData linked to the given ArticleData
        List<TagData> GetTagDataByArticleData(int id);                  // to get the TagData linked to the ArticleData with the given Id
    }
}
