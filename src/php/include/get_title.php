<?
include_once ("include/rearrange_title.php");

function get_title($show_id) {
    $title_select = mysql_query("SELECT title,year FROM shows WHERE ID=$show_id");
    while ($title_list = mysql_fetch_array($title_select)) {
	$title = stripslashes($title_list[0]);
	$title=rearrange_title($title);
	$year = $title_list[1];
    }
    $final = "$title ($year)";
    return ($final);
}
?>
