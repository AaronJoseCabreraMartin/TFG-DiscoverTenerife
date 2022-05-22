import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import AboutView from '../views/AboutView.vue'
import RankingPlayersView from '../views/RankingPlayersView.vue'
import RankingPlacesView from '../views/RankingPlacesView.vue'
import NotFoundView from '../views/404View.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/rankingplayers',
    name: 'rankingplayers',
    component: RankingPlayersView
  },
  {
    path: '/rankingplaces',
    name: 'rankingplaces',
    component: RankingPlacesView
  },
  {
    path: '/about',
    name: 'about',
    component: AboutView
  },
  //404
  {
    path: '/:catchAll(.*)',
    name: '404Error',
    component: NotFoundView
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
