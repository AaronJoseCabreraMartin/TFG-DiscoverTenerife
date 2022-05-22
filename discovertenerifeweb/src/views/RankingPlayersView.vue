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
        <tr v-for="(row, index) in rankingUser" :key="row.id">
          <template v-if="userMatchFilters(row)">
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
      rankingPlayers: [{//Modelo de datos
        uid: '',
        'Nombre de usuario': '',
        Puntuación: '',
        Rango: ''
      }],
      columns: [
        'Nombre de usuario',
        'Puntuación',
        'Rango'
      ],
      column: null,
      search: null
    }
  },
  methods: {
    async downloadRanking() {
      const authorization = firebase.auth();
      //console.log(authorization.currentUser);
      if (authorization.currentUser == null) {
        await authorization.signInAnonymously();
      }

      const snapshotUserUids = (await db.ref("users/usersThatAllowAppearedOnRanking").get()).val();
      let rankingList = [];
      //console.log(snapshotUserUids.val());
      for (let index = 0; index < snapshotUserUids.length; index++) {
        const uid = snapshotUserUids[index];
        const snapshotScore = await db.ref("users/" + uid + "/score_").get();
        const snapshotDisplayName = await db.ref("users/" + uid + "/displayName_").get();

        //console.log(snapshotDisplayName.val());

        rankingList.push({
          "uid": uid,
          'Puntuación': parseInt(snapshotScore.val() == null ? 0 : snapshotScore.val()),
          'Nombre de usuario': snapshotDisplayName.val(),
          'Rango': CalculateRange(snapshotScore.val())
        });
      }

      rankingList.sort((a, b) => {
        //queremos que ordene de mayor a menor score
        return b['Puntuación'] - a['Puntuación'];
      });
      //console.log(rankingList);

      this.loading = false;
      this.rankingUser = rankingList;
    },
    userMatchFilters(user) {
      if (this.column !== null && this.search !== null) {
        return typeof user[this.column] === 'string' ?
          user[this.column].toLowerCase().includes(this.search.toLowerCase())
          : user[this.column].toString() === this.search;
      }
      return true;
    }
  }, beforeMount() {
    this.downloadRanking()
  }
}

const CalculateRange = function (score) {
  return ranges.reduce((selectedRange, actualRange) => {
    if (actualRange.score_ <= score && selectedRange.score_ < actualRange.score_) {
      selectedRange = actualRange;
    }
    return selectedRange;
  }, {
    "range_": "undefined",
    "score_": -1
  })["range_"];
}

const ranges = [
  {
    "range_": "Mencey",
    "score_": 50000
  }, {
    "range_": "Achimencey",
    "score_": 10000
  }, {
    "range_": "Guañameñe",
    "score_": 3000
  }, {
    "range_": "Tagorero",
    "score_": 1500
  }, {
    "range_": "Sigoñes",
    "score_": 750
  }, {
    "range_": "Cichiciquitzos",
    "score_": 300
  }, {
    "range_": "Achicaxna",
    "score_": 100
  }, {
    "range_": "Achicaxnais",
    "score_": 0
  }
]

</script>