// import type { ConfirmationOptions } from 'primevue/confirmationoptions';

// type ConfirmType = {
//   require: (option: ConfirmationOptions) => void;
//   close: () => void;
// };

// let _confirm: ConfirmType;

// export function initConfirm(confirm: ConfirmType) {
//   _confirm = confirm;
// }

// export async function confirmDialog(
//   message: string,
//   header: string,
//   icon: string = 'pi pi-info-circle'
// ): Promise<boolean> {
//   return new Promise<boolean>(resolve => {
//     if (!_confirm) {
//       throw 'Confirm no init';
//     }
//     _confirm.require({
//       message,
//       header,
//       icon,
//       accept: () => resolve(true),
//       reject: () => resolve(false)
//     });
//   });
// }

import {
  type ConfirmDialog,
  type PromptDialog,
  useDialogStore
} from '@/store/dialog';

const store = useDialogStore();

export async function confirmDialog(message: string, title?: string) {
  return new Promise<boolean>(resolve => {
    const data: ConfirmDialog = {
      dialog: true,
      message,
      title,
      resolve
    };
    store.setConfirmDialog(data);
  });
}

export async function promptDialog(
  message: string,
  title?: string,
  validate?: (value?: string) => string | boolean
) {
  return new Promise<string | undefined>(resolve => {
    const data: PromptDialog = {
      dialog: true,
      message,
      title,
      validate,
      resolve
    };

    store.setPromptDialog(data);
  });
}
