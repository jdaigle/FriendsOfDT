<?
function additem($toadd,$level,$var1,$var2) {
    min_auth_level($level,'Not Authorized');
    #Create a new row in table
    $thingtoadd = mysql_real_escape_string($toadd);
    mysql_query ("insert into $thingtoadd () values ()");
    $new_id_query = mysql_query ("select max(ID) from $thingtoadd");
    while ($new_id = mysql_fetch_array($new_id_query)) { $id=$new_id[0]; }
    #edit that record
    $to_do = "edit" . $thingtoadd;
    $to_include = "include/" . $to_do . ".php";
    include_once ("$to_include");
    $to_do($id,$var1,$var2);
}

function delitem($todel,$ID) {
    min_auth_level(4,'Not Authorized');
    #Create a new row in table
    $thingtodel = mysql_real_escape_string($todel);
    $bkup = $thingtodel . "_bkup";
    $copy = mysql_query ("INSERT INTO $bkup select * from $thingtodel where ID=$ID");
    if(!$copy){ echo "There's little problem with the copy: ".mysql_error(); endlines(); }
    $delete = mysql_query ("DELETE FROM $thingtodel WHERE ID=$ID");
    if(!$delete){ echo "There's little problem with the deletion: ".mysql_error(); endlines(); }
    $update = mysql_query ("update $bkup set last_mod=CURRENT_DATE where ID=$ID");
    if(!$update){ echo "There's little problem with the update: ".mysql_error(); endlines(); }
}
?>
