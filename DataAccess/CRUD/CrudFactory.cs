using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD

{
    //Clase padre/madre abstracta de los crud
    //define como se hacen cruds en la arquitectura

    public abstract class CrudFactory
    {
        protected SqlDao _sqlDao;

        //Definir los metodos que forman parte del contrato
        //C = Create
        //R = Retieve
        //U = Update
        //D = Delte

        public abstract void Create(BaseDTO baseDTO);
        public abstract void Update(BaseDTO baseDTO);
        public abstract void Delte(BaseDTO baseDTO);

        public abstract T Retreive<T>;
        public abstract T RetrieveById<T>();
    }

}