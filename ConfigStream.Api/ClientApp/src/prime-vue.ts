import 'primevue/resources/themes/viva-dark/theme.css';
import 'primeicons/primeicons.css';
import 'primeflex/primeflex.css';
import PrimeVue from 'primevue/config';
import Button from 'primevue/button';
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
import ProgressSpinner from 'primevue/progressspinner';
import ToastService from 'primevue/toastservice';
import Toast from 'primevue/toast';
import type { App } from 'vue';

export function usePrimeVue(app: App) {
  app.component('Button', Button);
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
  app.component('ProgressSpinner', ProgressSpinner);
  app.component('Toast', Toast);
  app.use(ToastService);
  app.use(PrimeVue);
}
