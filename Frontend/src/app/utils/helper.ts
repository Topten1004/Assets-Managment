export const authorization = () => {
    return {
        headers: {
            Authorization: 'Bearer ' + window.localStorage.getItem('access_token'),
        }
    }
}

export const isAuthenticated = () => {
    if (getItem('access_token')) {
        return true;
    }
    return false;
}




export const setItem = (key: any, item: string) => {
  console.log(item)
    if (item) {
        window.localStorage.setItem(key, item);
    } else {
        window.localStorage.removeItem(key);
    }
}

export const removeItem = (key: any) => {
    if (key) return window.localStorage.removeItem(key);
}

export const getItem = (key: any) => {
    if (key) {
        return window.localStorage.getItem(key);
    } else {
      return null;
    }
}


export const formatDBDate = (db_date: any) => {

  if (typeof db_date === "undefined") return;

  let removeT_db_date = db_date.replace("T", " ");

  let removeTail_db_date = removeT_db_date.split(".")[0];

  return removeTail_db_date.toString();
}
