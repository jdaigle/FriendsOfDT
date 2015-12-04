<?

function delshow_check($show_id) {
    min_auth_level(5,'Not Authorized');
    include_once ("include/get_title.php");
    $totalrows = 0;
    $name = get_title($show_id);
    echo "<font size=+2>$name is currently associated with:</font><br>\n";

    $award_check = mysql_query("select ID from awards where showID=$show_id");
    $num_awards = mysql_num_rows($award_check);
    $totalrows = $totalrows + $num_awards;
    echo "$num_awards Award records<br>\n";

    $cast_check = mysql_query("select ID from cast where showID=$show_id");
    $num_cast = mysql_num_rows($cast_check);
    $totalrows = $totalrows + $num_cast;
    echo "$num_cast cast records<br>\n";

    $crew_check = mysql_query("select ID from crew where showID=$show_id");
    $num_crew = mysql_num_rows($crew_check);
    $totalrows = $totalrows + $num_crew;
    echo "$num_crew crew records<br>\n";

    if ($totalrows > 0) {
	echo "Please remove or re-associate the existing rows before deleting this show.<br>";
    } else {
	echo <<< EOT1
	<table><tr><td align=center>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='ID' value='$show_id'>
	<input type='hidden' name='action' value='delshow'>
	<input type=submit value='Delete Show'>
	</td><td>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type='hidden' name='action' value='show_detail'>
	<input type=submit value='Cancel'>
	</td></tr>
	</form>
	</table>
EOT1;
    }


}

?>
