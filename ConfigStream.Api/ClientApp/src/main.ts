import './assets/main.scss';
import 'primevue/resources/themes/viva-dark/theme.css';
import 'primeicons/primeicons.css';
import 'primeflex/primeflex.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './app.vue';
import router from './router';

import PrimeVue from 'primevue/config';
import Button from 'primevue/button';
import Card from 'primevue/card';
import TreeTable from 'primevue/treetable';
import Column from 'primevue/column';
import Chip from 'primevue/chip';
import Chips from 'primevue/chips';
import InputText from 'primevue/inputtext';
import MultiSelect from 'primevue/multiselect';
import Dropdown from 'primevue/dropdown';
import Toolbar from 'primevue/toolbar';
import Dialog from 'primevue/dialog';
import TieredMenu from 'primevue/tieredmenu';

const app = createApp(App);
app.component('Button', Button);
app.component('Card', Card);
app.component('TreeTable', TreeTable);
app.component('Column', Column);
app.component('Chip', Chip);
app.component('Chips', Chips);
app.component('InputText', InputText);
app.component('MultiSelect', MultiSelect);
app.component('Dropdown', Dropdown);
app.component('Toolbar', Toolbar);
app.component('Dialog', Dialog);
app.component('TieredMenu', TieredMenu);

app.use(PrimeVue);
app.use(createPinia());
app.use(router);
app.mount('#app');
