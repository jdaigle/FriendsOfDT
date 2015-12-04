<?

include_once ("include/castcrew.php");
include_once ("include/get_fullname.php");
include_once ("include/get_pic.php");

function show_peep_detail($peep_id,$editP) {
#    $peep_id = $_REQUEST[peep_id];
    $peep_select = mysql_query("select bio,username from people where ID=$peep_id");
    while ($result = mysql_fetch_array($peep_select)) {
	$wholename = get_fullname($peep_id);
	$biodata = str_replace("\n", "<br>", $result[0]);
	$uname = $result[1];
	$image_link= get_thumb_link('people',$peep_id);
	
	echo <<< EOT4
	<table align=center border=0>
	<tr><td></td><td>
	<center><font size=+3><b>$wholename</b></font><br><br> </center>
	</td></tr>
	<tr>
	<td align=center valign=top rowspan=3 width='30%'>
	$image_link
	<br>Click picture to see original size<br><br>
	<a href="$_SERVER[PHP_SELF]?action=media_index&amp;IDtype=people&amp;ID=$peep_id">See all media associated with this person</a><br><br>
EOT4;
	if ( ( isset ($_SESSION['username']) && ($_SESSION['username'] == $uname)) || ($_SESSION['level'] >= 1) ) {
	echo <<< EOT4
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editpeep'>
	<input type='hidden' name='peep_id' value='$peep_id'>
	<input type=submit value='Edit Personal Details'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]' target='_blank'>
	<input type='hidden' name='action' value='detail_page'>
	<input type='hidden' name='peep_id' value='$peep_id'>
	<input type=submit value='Printable Resume'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='addEC'>
        <input type='hidden' name='peep_id' value='$peep_id'>
        <input type=submit value='Add EC Position'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='addawards'>
        <input type='hidden' name='peep_id' value='$peep_id'>
        <input type=submit value='Add Award'>
        </form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='peep_detail'>
        <input type='hidden' name='peep_id' value='$peep_id'>
EOT4;
	if ($editP == 'edit') {
	    echo "	<input type=submit value='Hide Edit View'>\n";
	} else {
	    echo "	<input type='hidden' name='view' value='edit'>\n";
	    echo "	<input type=submit value='Show Edit View'>\n";
	}
        echo "	</form><br>\n";
	}
	if ( ( isset ($_SESSION['username']) && ($_SESSION['username'] == $uname)) || ($_SESSION['level'] >= 5) ) {
	    echo <<< EOT6
	<form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='delpeep_check'>
        <input type='hidden' name='peep_id' value='$peep_id'>
        <input type=submit value='Delete this Person'>
        </form><br>
EOT6;
	}
	echo <<< EOT4
	</td><td valign=top>
	   <table width=100% border=0 cellspacing=0 cellpadding=0>
EOT4;
	if (strlen($biodata) > 4 ) {
	   echo "<tr align='center'><td colspan=5><b><u>Biographical Data</u></b><br><br>$biodata<br><br>\n";
    	   echo "</td></tr>";
	}
    }
    castcrew($peep_id,$editP);
}

?>
