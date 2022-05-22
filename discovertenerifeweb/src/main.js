import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import {rtdbPlugin} from 'vuefire'

createApp(App).use(router).use(rtdbPlugin).mount('#app')
