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
