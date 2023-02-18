
var savedText = document.getElementById("note_text").innerHTML;

function NotSaved()
{
    let saveSpan = document.getElementById("save_confirm");

    saveSpan.innerHTML = "unsaved*";
    saveSpan.style.color = "#fe1111";

}