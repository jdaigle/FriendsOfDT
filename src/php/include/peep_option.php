<?

include_once ("include/get_listname.php");

function peep_option($selected_peep) {
    echo "<select name='peep_id'><option value=''></option>\n";
    $peep_select = mysql_query("SELECT ID FROM people where lname is not null or fname is not null ORDER BY lname,fname");
    while ($peeps = mysql_fetch_array($peep_select)) {
	$peep_id=$peeps[0];
	$listname= get_listname($peep_id);
	if (isset($selected_peep) && ($selected_peep==$peep_id)) {
	    echo "	<option selected value='$peep_id'>$listname</option>\n";
	} else {
	    echo "	<option value='$peep_id'>$listname</option>\n";
	}
    }
    echo "</select>\n";
}

?>
