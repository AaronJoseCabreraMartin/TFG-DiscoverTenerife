<template>
  <template v-if="loading">
        <div class="loader"></div>
  </template>

  <template v-else>
    <form class="form-horizontal">
      <div class="row">
        <select v-model="column" class="custom-select col-md-6">
          <option :value="null">No Filtrar</option>
          <option v-for="col in columns" :key="col">{{ col }}</option>
        </select>
        <input class="form-control col-md-6" type="text" v-model="search" placeholder="Search">
      </div>
    </form>


    <table class="table table table-bordered table-hover">
      <thead>
        <th>Top</th>
        <th v-for="col in columns" :key="col">{{ col }}</th>
      </thead>

      <tbody>
        <tr v-for="(row, index) in rankingPlaces" :key="row.id">
          <template v-if="placeMatchFilters(row)">
            <td>{{ index + 1 + 'º' }}</td>
            <td v-for="col in columns" :key="col">{{ row[col] }}</td>
          </template>
        </tr>
      </tbody>
    </table>
  </template>
</template>

<script>
const config = {
  apiKey: "AIzaSyB2OQ4uFnZNZBQYwbO8TbpIcrKBoRcaI2o",
  authDomain: "discovertenerife-fd031.firebaseapp.com",
  databaseURL: "https://discovertenerife-fd031-default-rtdb.europe-west1.firebasedatabase.app",
  projectId: "discovertenerife-fd031",
  storageBucket: "discovertenerife-fd031.appspot.com",
  messagingSenderId: "993595598765",
  appId: "1:993595598765:web:bc1a0374b627669f55e324",
  measurementId: "G-85ZYD5J6GM"
};

import firebase from 'firebase/compat/app'
import 'firebase/compat/database'
import 'firebase/compat/auth'

const firebaseApp = firebase.initializeApp(config);
const db = firebaseApp.database();

export default {
  data() {
    return {
      loading: true,
      rankingPlaces: [{//Modelo de datos
        Top: '',
        id: '',
        Tipo: '',
        Nombre: '',
        Zona: '',
        'Visitas acumuladas': ''
      }],
      columns: [
        'Nombre',
        'Tipo',
        'Zona',
        'Visitas acumuladas'
      ],
      column: null,
      search: null,
      downloadedData: []
    }
  },
  methods: {
    async downloadRanking() {
      const authorization = firebase.auth();
      //console.log(authorization.currentUser);
      if (authorization.currentUser == null) {
        await authorization.signInAnonymously();
      }
      this.downloadedData = (await db.ref("places").get()).val();
      this.fillRanking();
      this.loading = false;
    },

    fillRanking() {
      this.rankingPlaces = [];
      for (const type in this.downloadedData) {
        if (this.downloadedData.hasOwnProperty(type)) {
          this.downloadedData[type].forEach((place, index) => {
            this.rankingPlaces.push({
              id: index,
              Tipo: translatePorperty(type),
              Nombre: place.name_,
              Zona: translatePorperty(place.zone_),
              'Visitas acumuladas': place.timesItHasBeenVisited_ == null ? 0 : parseInt(place.timesItHasBeenVisited_)
            });
          });
        }
      }

      this.rankingPlaces.sort((a, b) => {
        //queremos que ordene de mayor a menor score
        return b['Visitas acumuladas'] - a['Visitas acumuladas'];
      });
    },

    placeMatchFilters(place) {
      if (this.column !== null && this.search !== null) {
        return typeof place[this.column] === 'string' ?
          place[this.column].toLowerCase().includes(this.search.toLowerCase())
          : place[this.column].toString() === this.search;
      }
      return true;
    }
  }, beforeMount() {
    this.downloadRanking();
  }
}

const translatePorperty = function (propertyName) {
  if (propertyName === 'naturalPools') {
    return 'Piscinas Naturales';
  } else if (propertyName === 'beaches') {
    return 'Playas';
  } else if (propertyName === 'viewpoints') {
    return 'Miradores';
  } else if (propertyName === 'hikingRoutes') {
    return 'Rutas de senderismo';
  } else if (propertyName === 'naturalParks') {
    return 'Parques naturales';
  } else if (propertyName === 'South') {
    return 'Sur';
  } else if (propertyName === 'North') {
    return 'Norte';
  } else if (propertyName === 'West') {
    return 'Oeste';
  } else if (propertyName === 'East') {
    return 'Este';
  } else if (propertyName === 'Center') {
    return 'Centro';
  }
  return propertyName;
}

</script>