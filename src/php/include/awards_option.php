<?
function awards_option($selected_award) {
    echo "<select name='ID'><option value=''></option>\n";
    $award_select = mysql_query("SELECT ID,name FROM awards_list order by name");
    while ($awards = mysql_fetch_array($award_select)) {
	$name=stripslashes($awards[1]);
	echo "      <option value='$awards[0]'";
	if (isset($selected_award) && ($selected_award==$awards[0])) { echo " selected"; }
	echo ">$name</option>\n";
    }
    echo "</select>\n";
}

?>
