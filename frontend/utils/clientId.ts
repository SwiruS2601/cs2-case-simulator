import { v4 as uuidv4 } from 'uuid';

const CLIENT_ID_KEY = 'client_id';

export function getClientId() {
  const storedId = localStorage.getItem(CLIENT_ID_KEY);
  if (storedId) {
    return storedId;
  }

  const uuid = uuidv4();
  localStorage.setItem(CLIENT_ID_KEY, uuid);

  return uuid;
}
