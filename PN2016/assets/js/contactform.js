
var data = {
    "kovils": {
        "Ilayatrangudi": ["Kazhani Vaasarkkudaiyar","Kinginikkoorudaiyar","Okkurudaiyar","Pattanasamiyar","Perusenthurudaiyar","Sirusenthurudaiyar","Perumaruthurudaiyar"],
        "Mathur": ["Arumbakkur", "Kannur", "Karuppur", "Kulathur", "Mannur", "Manalur", "Uraiyur"],
        "Vairavan Kovil": ["Kazhani Vaasarkkudaiyar","Maruthenthirapuram","Periya vahuppu","Pilliyar vahuppu","Theyyanar vahuppu"]
    }
}

$(document).ready(
    function () {
                
        $("#nativePlace").autocomplete({
            source: nativePlaceList
        });

        $("#spouseNativePlace").autocomplete({
            source: nativePlaceList
        });

        if (kovil != '')
        {
            $("#kovil").val(kovil);
            LoadPirivu(kovil, 'kovilPirivu');
            $("#kovilPirivu").val(kovilPirivu);
        }
        if(maritalStatus != '')
        {
            ToogleSpouseForm(maritalStatus);
            if (skovil != '') {
                $("#spouseKovil").val(skovil);
                LoadPirivu(skovil, 'spouseKovilPirivu');
                $("#spouseKovilPirivu").val(skovilPirivu);
            }
        }
    }
);

//Image Preview
document.getElementById("f1.Image").onchange = function () {
    var reader = new FileReader();

    reader.onload = function (e) {
        // get loaded data and render thumbnail.
        document.getElementById("f1.ImagePreview").src = e.target.result;
    };

    // read the image file as a data URL.
    reader.readAsDataURL(this.files[0]);
};

//Load Kovil Pirivu based on Kovil
function LoadPirivu(kovilname, target)
{
    var pirivus = data.kovils[kovilname];
    var selectHtml = "<option value=''> Choose Pirivu</option>";
    if (pirivus)
    {
        for (var i = 0; i < pirivus.length; i++) {
            selectHtml += "<option value='" + pirivus[i] + "'>" + pirivus[i] + "</option>";
        }
    }
    else
    {
        var selectHtml = "<option value='NoPirivu'> No Pirivu</option>";
    }
    document.getElementById(target).innerHTML = selectHtml;
}

function ToogleSpouseForm(mstatus)
{
    var secForm = document.getElementById('spouse');
    if (mstatus != 'S')
    {
        secForm.style.display = "block";
        SpouseFormValidation(true);
    }
    else
    {
        secForm.style.display = "none";
        SpouseFormValidation(false);
    }
    KidsReset();
}

function AddMore()
{
    document.getElementById('addKids').style.display = "none";
    document.getElementById('kids+').style.display = "block";
}

function KidsReset()
{
    document.getElementById('addKids').style.display = "block";
    document.getElementById('kids+').style.display = "none";
}

function SpouseFormValidation(status)
{
    var fields = ["spouseFirstName", "spouseLastName", "spouseKovil","spouseKovilPirivu", "spouseNativePlace"];
    for (var index in fields)
    {
        var field = fields[index];
        document.getElementById(field).required = status;
    };
}