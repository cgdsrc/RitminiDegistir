﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takas.Common.Entities.Concrete;

namespace Takas.Business.Abstract
{
    public interface IBrandService
    {
        /// <summary>
		/// Bu Method VeriTabanindaki Tum Categorilerin Listesini Getitir.
		/// </summary>
		/// <returns>Category List</returns>
		Task<List<Brand>> GetList();

        /// <summary>
        /// Veritabanindan ARANAN Category kaydını doner.
        /// </summary>
        /// <param name="id"> Verilen Id parametresine gore veritabaninda arama yapilir</param>
        /// <returns>Category</returns>
        Brand Get(int id);

        /// <summary>
        /// Veritabanina Yeni bir Category Eklemek icin bu method kullanilir.
        /// </summary>
        /// <param name="entity">Bu Parametre Category Türünde bir nesnedir.</param>
        void Add(Brand entity);

        /// <summary>
        /// Veritabanindaki kaydi gelen kayida gore gunceller.
        /// </summary>
        /// <param name="entity">Category Turunde bir degisken gonderiliyor ve bu degiskene gore Category tablosuna ekleme yapmasi isteniyor.</param>
        void Update(Brand entity);

        /// <summary>
        /// Veritabanindaki Category tablosundan tek bir kayit silinmaktadir.
        /// </summary>
        /// <param name="entity">Category türünde bir nesne veriyoruz ve veritabanından o nesne siliniyor.</param>
        void Delete(Brand entity);
    }
}
