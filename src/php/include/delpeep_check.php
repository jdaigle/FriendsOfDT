<?


function delpeep_check($peep_id) {
    min_auth_level(5,'Not Authorized');
    include_once ("include/get_fullname.php");
    $totalrows = 0;
    $name = get_fullname($peep_id);
    echo "<font size=+2>$name is currently associated with:</font><br>\n";

    $award_check = mysql_query("select ID from awards where peepID=$peep_id");
    $num_awards = mysql_num_rows($award_check);
    $totalrows = $totalrows + $num_awards;
    echo "$num_awards Award records<br>\n";

    $cast_check = mysql_query("select ID from cast where peepID=$peep_id");
    $num_cast = mysql_num_rows($cast_check);
    $totalrows = $totalrows + $num_cast;
    echo "$num_cast cast records<br>\n";

    $crew_check = mysql_query("select ID from crew where peepID=$peep_id");
    $num_crew = mysql_num_rows($crew_check);
    $totalrows = $totalrows + $num_crew;
    echo "$num_crew crew records<br>\n";

    $EC_check = mysql_query("select ID from EC where peepID=$peep_id");
    $num_EC = mysql_num_rows($EC_check);
    $totalrows = $totalrows + $num_EC;
    echo "$num_EC EC records<br>\n";

    if ($totalrows > 0) {
	echo "Please remove or re-associate the existing rows before deleting this person.<br>";
    } else {
	echo <<< EOT1
	<table><tr><td align=center>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='ID' value='$peep_id'>
	<input type='hidden' name='action' value='delperson'>
	<input type=submit value='Delete Person'>
	</td><td>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='peep_id' value='$peep_id'>
	<input type='hidden' name='action' value='peep_detail'>
	<input type=submit value='Cancel'>
	</td></tr>
	</form>
	</table>
EOT1;
    }


}

?>
