<?

include_once ("include/rearrange_title.php");

function show_option($selected_show) {
    echo "<select name='show_id'><option value=''></option>\n";
    $show_select = mysql_query("SELECT ID,title,year FROM shows where title is not null ORDER by title,year");
    while ($shows = mysql_fetch_array($show_select)) {
	$title=stripslashes($shows[1]);
	$title=rearrange_title($title);
	echo "      <option value='$shows[0]'";
	if (isset($selected_show) && ($selected_show==$shows[0])) { echo " selected"; }
	echo ">$title ($shows[2])</option>\n";
    }
    echo "</select>\n";
}

?>
