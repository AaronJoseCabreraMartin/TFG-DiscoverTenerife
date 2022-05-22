<template>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
  <nav class="navbar navbar-dark bg-dark">
    <router-link to="/">Home</router-link> |
    <router-link to="/rankingplayers">Player Ranking</router-link> |
    <router-link to="/rankingplaces">Places Ranking</router-link> |
    <router-link to="/about">About</router-link>
  </nav>
  <!--ESTE SE EJECUTA CON NPM RUN SERVE
    App.vue son los elementos que tendran todas las paginas que cuelgen de nuestra
   pagina web y routerview sera sustituido por las vistas que le digamos en el router/index.js-->
  <router-view/>
</template>

<style>

* {
  background-color: #abceecbb;
}

#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  font-size: 2vh;
}

nav {
  padding: 30px;
}

nav a {
  font-weight: bold;
  color: #2f7e5a;
}

nav a.router-link-exact-active {
  color: #42b983;
}

select {
  text-align: center;
  padding: 0px 10vw;
}

h1 {
  padding: 2vh 2vw;
  font-size: 10vw;
}

h3 {
  padding: 2vh 2vw;
}

p {
  padding: 1vh 1vw;
}

.loader {
  border: 16px solid #f3f3f3; /* Light grey */
  border-top: 16px solid #3498db; /* Blue */
  border-radius: 50%;
  width: 25vh;
  height: 25vh;
  animation: spin 2s linear infinite;

  position: fixed;
  top: 30%;
  left: 40%;
  transform: translate(-50%, -50%);
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
