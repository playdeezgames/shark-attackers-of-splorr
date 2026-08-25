function loadGame(filename) {
    try {
        let saveData = localStorage.getItem(filename)
        if (saveData == null) {
            return null;
        }
        return saveData
    } catch (ex) {
        console.log(ex);
        return null;
    }
}
function saveGame(filename, data) {
    localStorage.setItem(filename, data);
}