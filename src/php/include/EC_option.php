<?
function EC_option($selected_EC) {
    echo "<select name='ID'><option value=''></option>\n";
    $EC_select = mysql_query("SELECT ID,title FROM EC_list order by title");
    while ($EC = mysql_fetch_array($EC_select)) {
	$name=stripslashes($EC[1]);
	echo "      <option value='$EC[0]'";
	if (isset($selected_EC) && ($selected_EC==$awards[0])) { echo " selected"; }
	echo ">$name</option>\n";
    }
    echo "</select>\n";
}
?>
