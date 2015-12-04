<?
include_once ("include/get_q.php");

function q_option ($q) {
    for ($i=1; $i < 5; $i+=1) {
        $q_name = get_q($i);
	if ($i == $q) { echo "<option value='$i' selected>$q_name</option>\n";}
	else {echo "<option value='$i'>$q_name</option>\n";}
    }
}

?>
